using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Game;
using GameLibrary;
using Microsoft.Win32;

namespace GnomeExtractor
{
    public partial class MainWindow : Window
    {
        bool isDebugMode = false; // Changing this in project page didn't give any result
        bool isWindowsXP = false;
        bool isLabelsVertical;
        string filePath;
        string lastBackupFileName;
        string[] dataGridNames = new string[] { "dataGridProfessions", "dataGridCombat", "dataGridAttributes" };

        Task task;
        Statistics mapStatistics;
        GnomanEmpire gnomanEmpire;
        //ResourceManager resourceManager;
        Settings settings = new Settings();
        BackgroundWorker bkgw = new BackgroundWorker();
        BackgroundWorker updater = new BackgroundWorker();
        BackgroundWorker backgrWorker = new BackgroundWorker();
        OperatingSystem osInfo = Environment.OSVersion;

        #region WindowHandling
        public MainWindow()
        {
            Globals.Logger.Info("Gnome Extractor is running...");

            if (osInfo.Version.Major == 5 && osInfo.Version.Minor != 0)
                isWindowsXP = true;

            // Read settings from Xml file
            settings.ReadXml();

            // Write english sample file for interpreters
            if (isDebugMode) Globals.WriteEnglishXml();

            // Read languages
            Globals.LoadLanguages();

            // При первом запуске выставляем культуру установленную в компе, при последующих - предыдущую
            // First run changing localization same like in computer
            Globals.ViewModel.Language = Globals.ViewModel.Languages[0];
            foreach (var language in Globals.ViewModel.Languages)
                if (settings.Fields.ProgramLanguage == language.LanguageTitle) Globals.ViewModel.Language = language;

            Globals.Logger.Debug("Language selected: {0}", Globals.ViewModel.Language.LanguageTitle);

            InitializeComponent();

            //UpdateLanguageMenus();

            // Загружаем настроечки с прошлого запуска
            // Loading settings
            this.WindowState = settings.Fields.LastRunWindowState;
            this.Left = settings.Fields.LastRunLocation.X;
            this.Top = settings.Fields.LastRunLocation.Y;
            this.Width = settings.Fields.LastRunSize.Width;
            this.Height = settings.Fields.LastRunSize.Height;
            Globals.ViewModel.IsCheatsEnabled = settings.Fields.LastRunCheatMode;
            this.isLabelsVertical = settings.Fields.LastRunIsLablesVertical;
            this.tabControl.SelectedIndex = settings.Fields.TabItemSelected;

            Globals.Logger.Debug("Settings have been loaded");
            Globals.Logger.Debug("Game initialization...");
            typeof(GnomanEmpire).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(GnomanEmpire.Instance, null);
            if (GnomanEmpire.Instance.Graphics.IsFullScreen) GnomanEmpire.Instance.Graphics.ToggleFullScreen();
            GnomanEmpire.Instance.AudioManager.MusicVolume = 0;
            Globals.Logger.Debug("Game initialized");

            Globals.Initialize();

            DataContext = Globals.ViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ControlStates();

            Globals.Logger.Info("Running is complete");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Globals.Logger.Debug("Trying to close program...");
            if (((task != null) && !task.IsCompleted) || backgrWorker.IsBusy || progressBarMain.IsVisible) e.Cancel = true;
            else
            {
                Globals.Logger.Info("Program closes...");
                settings.Fields.ProgramLanguage = Globals.ViewModel.Language.LanguageTitle;
                settings.Fields.LastRunWindowState = this.WindowState;
                settings.Fields.LastRunLocation = new Point(Left, Top);
                settings.Fields.LastRunSize = new Size(Width, Height);
                settings.Fields.LastRunCheatMode = Globals.ViewModel.IsCheatsEnabled;
                settings.Fields.LastRunIsLablesVertical = !this.isLabelsVertical;
                settings.Fields.TabItemSelected = tabControl.SelectedIndex;
                settings.WriteXml();
                Globals.Logger.Debug("Settings have been saved");
                if (gnomanEmpire != null) gnomanEmpire = null;

                Globals.Logger.Info("Program is closed");
            }
        }
        #endregion

        #region ButtonClickHandlers
        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Debug("About button has been clicked");
            About about = new About();
            about.DataContext = Globals.ViewModel;
            about.Owner = this;
            about.Show();
        }

        private void fastEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FastEditor fastEditor = new FastEditor();
            fastEditor.DataContext = Globals.ViewModel;
            fastEditor.Owner = this;
            fastEditor.ShowDialog();

            if (fastEditor.IsOkClicked)
            {
                Globals.Logger.Trace("FastEditor OK button has been clicked");

                if (fastEditor.fixedModeRadioButton.IsChecked == true)
                {
                    foreach (var gnome in Globals.ViewModel.Gnomes)
                    {
                        if (fastEditor.isLaborNeededCheckBox.IsChecked == true)
                            foreach (var skill in gnome.LaborSkills)
                                skill.Level = fastEditor.Value;
                        if (fastEditor.isCombatNeededCheckBox.IsChecked == true)
                            foreach (var skill in gnome.CombatSkills)
                                skill.Level = fastEditor.Value;
                        if (fastEditor.isAttributesNeededCheckBox.IsChecked == true)
                            foreach (var attribute in gnome.Attributes)
                                attribute.Level = fastEditor.Value;
                    }
                }
            }
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Debug("Open button has been clicked");

            if (isWindowsXP && !File.Exists("fixed7z.dll"))
            {
                File.Copy("7z.dll", "fixed7z.dll");
                Globals.Logger.Debug("Fixed 7z.dll has been created");
                SevenZip.SevenZipExtractor.SetLibraryPath("fixed7z.dll");
            }
            else
                SevenZip.SevenZipExtractor.SetLibraryPath("7z.dll");

            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.InitialDirectory = System.IO.Path.GetFullPath(GnomanEmpire.SaveFolderPath("Worlds\\"));
            openDlg.Filter = "Gnomoria Save Files (*.sav)|*.sav";

            Globals.Logger.Debug("Open file dialog creating...");
            if (openDlg.ShowDialog() == true)
            {
                Globals.Logger.Debug("Open file dialog has been closed");

                DisableControlsForBackgroundWorker();
                filePath = openDlg.FileName;

                // Cleaning before loading
                Globals.ViewModel.Clean();

                Environment.CurrentDirectory = Globals.ApplicationFolder;

                backgrWorker.DoWork += new DoWorkEventHandler(LoadGame_BackgroundWorker);
                backgrWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadGameCompleted_BackgroundWorker);
                backgrWorker.RunWorkerAsync(openDlg.SafeFileName);
            }
        }

        private void LoadGameCompleted_BackgroundWorker(object sender, RunWorkerCompletedEventArgs e)
        {
            mapStatistics = new Statistics(gnomanEmpire);
            Globals.ViewModel.Statistics = mapStatistics;

            GridState();
            backgrWorker.DoWork -= new DoWorkEventHandler(LoadGame_BackgroundWorker);
            backgrWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadGameCompleted_BackgroundWorker);

            statusBarLabel.Content = Globals.ViewModel.Language.chosenFile + " " + System.IO.Path.GetFileName(filePath) +
                    " " + Globals.ViewModel.Language.worldName + " " + gnomanEmpire.World.AIDirector.PlayerFaction.Name;

            Globals.Logger.Info("World initialization complete");
            ControlStates();
        }

        private void LoadGame_BackgroundWorker(object sender, DoWorkEventArgs e)
        {
            Globals.Logger.Info("World initialization...");
            Globals.Logger.Info("Loading {0} save file...", e.Argument);

            GnomanEmpire.Instance.LoadGame((string)e.Argument);

            gnomanEmpire = GnomanEmpire.Instance;

            // Looking for Professions
            Globals.ViewModel.Professions.Add(new Profession("Custom"));
            Globals.ViewModel.Professions[0].AllowedSkills.ClearAll();
            foreach (var profession in gnomanEmpire.Fortress.Professions)
                Globals.ViewModel.Professions.Add(profession);

            // Перебор гномов на карте
            // Looking for gnomes at the map
            var gnomeIndex = 0;
            for (int level = 0; level < gnomanEmpire.Map.Levels.Length; level++)
                for (int row = 0; row < gnomanEmpire.Map.Levels[0].Length; row++)
                    for (int col = 0; col < gnomanEmpire.Map.Levels[0][0].Length; col++)
                        for (int num = 0; num < gnomanEmpire.Map.Levels[level][row][col].Characters.Count; num++)
                            if (gnomanEmpire.Map.Levels[level][row][col].Characters[num].RaceID == RaceID.Gnome)
                            {
                                var gnome = gnomanEmpire.Map.Levels[level][row][col].Characters[num];

                                Globals.ViewModel.Gnomes.Add(new Gnome(gnome, level, row, col, num, gnomeIndex));

                                gnomeIndex++;
                            }

            // Changing unknown professions (Custom like) titles to Custom
            foreach (var gnome in Globals.ViewModel.Gnomes)
            {
                var found = false;
                foreach (var profession in Globals.ViewModel.Professions)
                    if (gnome.Profession.Title == profession.Title) found = true;
                if (!found) gnome.Profession.Title = "Custom";
            }
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Debug("Save button has been clicked");
            DisableControlsForBackgroundWorker();

            backgrWorker.DoWork += new DoWorkEventHandler(SaveGame_BackgroundWorker);
            backgrWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SaveGameCompleted_BackgroundWorker);
            backgrWorker.RunWorkerAsync();
        }

        private void SaveGameCompleted_BackgroundWorker(object sender, RunWorkerCompletedEventArgs e)
        {
            backgrWorker.DoWork -= new DoWorkEventHandler(SaveGame_BackgroundWorker);
            backgrWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(SaveGameCompleted_BackgroundWorker);

            task = gnomanEmpire.SaveGame().ContinueWith((antecedent) =>
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        statusBarLabel.Content = Globals.ViewModel.Language.saveDoneMessage + " " + gnomanEmpire.CurrentWorld + ", " +
                        Globals.ViewModel.Language.backupDoneMessage + " " + lastBackupFileName;

                        Globals.Logger.Info("Save complete");

                        ControlStates();
                    }));
            });
        }

        private void SaveGame_BackgroundWorker(object sender, DoWorkEventArgs e)
        {
            Globals.Logger.Info("World saving...");

            var dir = GnomanEmpire.SaveFolderPath("Backup\\");
            Directory.CreateDirectory(dir);
            lastBackupFileName = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" +
                                 DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + gnomanEmpire.CurrentWorld;
            File.Copy(filePath, dir + lastBackupFileName, true);
            Globals.Logger.Info("Backup file created at" + dir + lastBackupFileName);

            for (int i = gnomanEmpire.Fortress.Professions.Count; i > 0; i--)
                gnomanEmpire.Fortress.Professions.RemoveAt(i - 1);
            foreach (var profession in Globals.ViewModel.Professions)
                if (profession.Title != "Custom")
                    gnomanEmpire.Fortress.Professions.Add(profession);

            foreach (var unsavedGnome in Globals.ViewModel.Gnomes)
            {
                var gnome = gnomanEmpire.Map.Levels[unsavedGnome.Level][unsavedGnome.Row][unsavedGnome.Column].Characters[unsavedGnome.Position];


                // Setting personal info
                gnome.SetName(unsavedGnome.Name);

                // Setting skill/attribute levels
                foreach (var laborSkill in unsavedGnome.LaborSkills)
                    gnome.SetSkillLevel(laborSkill.Type, laborSkill.Level);
                foreach (var combatSkill in unsavedGnome.CombatSkills)
                    gnome.SetSkillLevel(combatSkill.Type, combatSkill.Level);
                foreach (var attribute in unsavedGnome.Attributes)
                    gnome.SetAttributeLevel(attribute.Type, (int)attribute.Level);

                // Setting professions
                unsavedGnome.UpdateAllowedSkills();
                gnome.Mind.Profession = unsavedGnome.Profession;

                Globals.Logger.Debug("Gnome {0} written", gnome.Name());
            }
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Debug("Exit button clicked");

            Close();
        }

        private void languageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var language = (sender as MenuItem).DataContext as LanguageDictonaryEntry;
            Globals.ViewModel.Language = language;
        }

        private void cheatModeMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Debug("Cheat mode button has been clicked");

            Globals.ViewModel.IsCheatsEnabled = !Globals.ViewModel.IsCheatsEnabled;
            ControlStates();
            GridState();

            Globals.Logger.Info("Cheat mode {0}", (Globals.ViewModel.IsCheatsEnabled) ? "enabled" : "disabled");
        }

        private void exportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Globals.Logger.Info("Export is running...");

            DisableControlsForBackgroundWorker();

            backgrWorker.DoWork += new DoWorkEventHandler(backgrWorker_ExportToCSV);
            backgrWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgrWorker_ExportToCSVCompleted);
            backgrWorker.RunWorkerAsync();
        }

        private void backgrWorker_ExportToCSV(object sender, DoWorkEventArgs e)
        {
            // Opening file stream
            Directory.CreateDirectory(Globals.AppDataPath);
            var path = Globals.AppDataPath + "\\export.csv";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            // Writing first row
            sw.Write("Name;Profession;");
            foreach (var skill in SkillDef.AllLaborSkills())
                sw.Write(skill.ToString() + ";");
            foreach (var skill in SkillDef.AllCombatSkills())
                sw.Write(skill.ToString() + ";");
            foreach (var attribute in Enum.GetValues(typeof(CharacterAttributeType)))
                if (attribute.ToString() != "Count")
                    sw.Write(attribute.ToString() + ";");

            // Writing gnomes
            foreach (var gnome in Globals.ViewModel.Gnomes)
            {
                sw.Write("\n" + gnome.Name + ";" + gnome.Profession.Title + ";");
                foreach (var skill in gnome.LaborSkills)
                    sw.Write(skill.Level + ";");
                foreach (var skill in gnome.CombatSkills)
                    sw.Write(skill.Level + ";");
                foreach (var attribute in gnome.Attributes)
                    if (attribute.Name != "Count")
                        sw.Write(attribute.Level + ";");
            }
            sw.Close();
            Globals.Logger.Info("File {0} is created", path);
        }

        private void backgrWorker_ExportToCSVCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgrWorker.DoWork -= new DoWorkEventHandler(backgrWorker_ExportToCSV);
            backgrWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgrWorker_ExportToCSVCompleted);

            Globals.Logger.Info("Export is complete");
            statusBarLabel.Content = Globals.ViewModel.Language.exportDoneMessage;

            Process.Start(Globals.AppDataPath);

            ControlStates();
        }
        #endregion

        #region State handling
        private void ControlStates()
        {
            cheatsMenuItem.Visibility = (settings.Fields.IsCheatsEnabled) ? Visibility.Visible : Visibility.Collapsed;
            if (!settings.Fields.IsCheatsEnabled) settings.Fields.LastRunCheatMode = false;
            fastEditMenuItem.IsEnabled = (gnomanEmpire != null && Globals.ViewModel.IsCheatsEnabled);
            saveMenuItem.IsEnabled = exportMenuItem.IsEnabled = professionsEditingGroupBox.IsEnabled = (gnomanEmpire != null && !backgrWorker.IsBusy);
            openMenuItem.IsEnabled = !(backgrWorker.IsBusy);
            cheatModeMenuItem.IsChecked = Globals.ViewModel.IsCheatsEnabled;
            //autoUpdatingMenuItem.IsChecked = isAutoUpdateEnabled;
            progressBarMain.Visibility = (backgrWorker.IsBusy) ? Visibility.Visible : Visibility.Hidden;
        }

        private void DisableControlsForBackgroundWorker()
        {
            fastEditMenuItem.IsEnabled = openMenuItem.IsEnabled = saveMenuItem.IsEnabled =
                exportMenuItem.IsEnabled = professionsEditingGroupBox.IsEnabled = false;
            progressBarMain.Visibility = System.Windows.Visibility.Visible;
        }

        private void GridState()
        {
            if (gnomanEmpire != null)
            {
                var tempIndex = tabControl.SelectedIndex;
                tabControl.SelectedIndex = 0;
                dataGridProfessions.UpdateLayout();
                for (int i = Globals.FirstColumnCount; i < dataGridProfessions.Columns.Count; i++)
                    dataGridProfessions.Columns[i].IsReadOnly = !Globals.ViewModel.IsCheatsEnabled;
                tabControl.SelectedIndex = 1;
                dataGridCombat.UpdateLayout();
                for (int i = Globals.FirstColumnCount; i < dataGridCombat.Columns.Count; i++)
                    dataGridCombat.Columns[i].IsReadOnly = !Globals.ViewModel.IsCheatsEnabled;
                tabControl.SelectedIndex = tempIndex;
            }
        }
        #endregion

        #region DataGrids handling (is not logged)
        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.DisplayIndex == Globals.FirstColumnCount - 1) e.Handled = true;

            if (e.Column.DisplayIndex > Globals.FirstColumnCount - 1)
                e.Column.SortDirection = ListSortDirection.Ascending;
        }

        private void DataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
            if (e.ChangedButton == MouseButton.Right || (e.ChangedButton == MouseButton.Left && !Globals.ViewModel.IsCheatsEnabled) && tabControl.SelectedIndex == 0)
            {
                // Getting indexes
                var cell = sender as DataGridCell;
                if (cell.Column.DisplayIndex > Globals.FirstColumnCount - 1)
                {
                    //var skillIndex = cell.Column.DisplayIndex - Globals.FirstColumnCount;
                    var skillIndex = dataGridProfessions.Columns.IndexOf(cell.Column) - Globals.FirstColumnCount;
                    var gnome = cell.DataContext as Gnome;
                    if (skillIndex > gnome.LaborSkills.Count) return;

                    // Changing skills
                    gnome.LaborSkills[skillIndex].IsAllowed = !gnome.LaborSkills[skillIndex].IsAllowed;
                    if (gnome.LaborSkills[skillIndex].IsAllowed)
                        gnome.Profession.AllowedSkills.AddSkill(gnome.LaborSkills[skillIndex].Type);
                    else
                        gnome.Profession.AllowedSkills.RemoveSkill(gnome.LaborSkills[skillIndex].Type);
                    var comboBox = professionsColumn.GetCellContent(DataGridRow.GetRowContainingElement(cell)) as ComboBox;
                    gnome.Profession.Title = "Custom";
                    comboBox.SelectedIndex = 0;
                }
            }
            else
            {
                //var cell = sender as DataGridCell;
                //cell.IsSelected = true;
                //cell.IsEditing = true;
            }
        }

        private void DataGridRowHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRowHeader header = sender as DataGridRowHeader;
            if (header != null)
            {
                DataGrid dataGrid = FindName(dataGridNames[tabControl.SelectedIndex]) as DataGrid;
                DataGridRow row = DataGridRow.GetRowContainingElement(sender as DataGridRowHeader);

                // Reading
                DictionaryEntry[] values = new DictionaryEntry[dataGrid.Columns.Count - Globals.FirstColumnCount];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i].Key = i + Globals.FirstColumnCount;
                    values[i].Value = Int32.Parse((dataGrid.Columns[i + Globals.FirstColumnCount].GetCellContent(row) as TextBlock).Text);
                }

                // Sorting
                for (int i = 0; i < values.Length - 1; i++)
                    for (int j = i + 1; j < values.Length; j++)
                        if ((int)values[i].Value < (int)values[j].Value)
                        {
                            var temp = values[i];
                            values[i] = values[j];
                            values[j] = temp;
                        }

                // Reordering
                for (int i = 0; i < values.Length; i++)
                {
                    dataGrid.Columns[(int)values[i].Key].DisplayIndex = i + Globals.FirstColumnCount;

                    e.Handled = true;
                }
            }
        }

        private void DataGridColumnHeaderProfessions_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var header = e.Source as DataGridColumnHeader;
            try
            {
                header.ToolTip = Globals.ViewModel.Language.GetType().GetProperty(header.Content.ToString().Replace(" ", String.Empty)).GetValue(Globals.ViewModel.Language, null);
            }
            catch { }
            //header.ToolTip = resourceManager.GetString(header.Content.ToString().Replace(" ", String.Empty));
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == Globals.FirstColumnCount - 1)
            {
                //var value = (e.EditingElement as ComboBox).SelectedValue.ToString();
            }

            // Проверяем является ли выделенная ячейка именем
            // Checking if cell is name
            if (e.Column.DisplayIndex == Globals.FirstColumnCount - 2)
            {
                var value = (e.EditingElement as TextBox).Text;

                if (value.Length > 24)
                    value = value.Substring(0, 24);

                (e.EditingElement as TextBox).Text = value;
            }
            if (e.Column.DisplayIndex > Globals.FirstColumnCount - 1)
            {
                var value = (e.EditingElement as TextBox).Text;

                long num;
                if (Int64.TryParse(value, out num))
                {
                    if (Int64.Parse(value) > 5000)
                        (e.EditingElement as TextBox).Text = "5000";
                    else if (Int64.Parse(value) < 5)
                        (e.EditingElement as TextBox).Text = "5";
                }
                else
                    (e.EditingElement as TextBox).Text = "5";
            }
        }

        private void DataGridProfessionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            if (dataGridProfessions.SelectedCells.Count == 0) return;
            var profession = (Profession)e.AddedItems[0];
            Globals.Logger.Trace("Professions combobox selection changed to {0}", profession.Title);
            var gnome = Globals.ViewModel.Gnomes[((Gnome)dataGridProfessions.SelectedCells[0].Item).ID];
            if (profession.Title == "Custom") profession.AllowedSkills = gnome.Profession.AllowedSkills;
            gnome.Profession = profession;
            gnome.SetAllowedSkills(profession.AllowedSkills);

            Globals.Logger.Trace("Gnome {0} profession has been changed to {1}", gnome.Name, Globals.ViewModel.Gnomes[0].Profession.Title);
        }
        #endregion

        #region Professions handling
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Globals.Logger.Trace("TabControl.SelectedIndex is changed to {0}", tabControl.SelectedIndex);
        }

        private void AddProfessionButton_Click(object sender, RoutedEventArgs e)
        {
            var name = professionsComboBox.Text;

            // Abort if already exists
            foreach (var profession in Globals.ViewModel.Professions)
            {
                if (profession.Title == name)
                {
                    MessageBox.Show(Globals.ViewModel.Language.professionAlreadyExists);
                    return;
                }
            }

            // Adding profession
            Profession newProfession = new Profession(name);
            newProfession.AllowedSkills.ClearAll();
            foreach (var skill in Globals.ViewModel.Skills)
                if (skill.IsAllowed)
                    newProfession.AllowedSkills.AddSkill(skill.Type);
            Globals.ViewModel.Professions.Add(newProfession);

            // Refreshing combobox
            professionsComboBox.ItemsSource = null;
            professionsComboBox.ItemsSource = Globals.ViewModel.Professions;
        }

        private void RemoveProfessionButton_Click(object sender, RoutedEventArgs e)
        {
            var profession = professionsComboBox.SelectedItem as Profession;
            if (profession.Title == "Custom") return;
            foreach (var gnome in Globals.ViewModel.Gnomes)
            {
                if (gnome.Profession.Title == profession.Title)
                {
                    var newProfession = new Profession("Custom");
                    newProfession.AllowedSkills = gnome.Profession.AllowedSkills;
                    gnome.Profession = newProfession;
                }
            }
            Globals.ViewModel.Professions.Remove(profession);

            professionsComboBox.ItemsSource = null;
            professionsComboBox.ItemsSource = Globals.ViewModel.Professions;
        }

        private void readGnomeButton_Click(object sender, RoutedEventArgs e)
        {
            var gnome = gnomesComboBox.SelectedItem as Gnome;

            if (gnome.Profession.Title == "Custom") professionsComboBox.SelectedIndex = 0;
            else professionsComboBox.SelectedItem = gnome.Profession;
            Globals.ViewModel.Skills = gnome.GetClonedSkills();

            professionsListBox.ItemsSource = null;
            professionsListBox.ItemsSource = Globals.ViewModel.Skills;
        }

        private void professionsComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".IndexOf(e.Text) < 0;
        }

        private void professionsComboBox_PreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void professionsComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.Insert))
                e.Handled = true;
        }

        private void professionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var profession = e.AddedItems[0] as Profession;
            if (profession.Title != "Custom" && Globals.ViewModel.Skills.Count > 1)
            {
                Globals.ViewModel.Skills.Clear();
                foreach (var skill in SkillDef.AllLaborSkills())
                    Globals.ViewModel.Skills.Add(new SkillEntry(skill, 5, profession.AllowedSkills.IsSkillAllowed(skill)));
            }

            professionsListBox.ItemsSource = null;
            professionsListBox.ItemsSource = Globals.ViewModel.Skills;
        }
        #endregion
    }
}