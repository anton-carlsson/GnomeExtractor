using System;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace GnomeExtractor
{
    public class SettingsFields
    {
        public bool LastRunCheatMode = false;
        public bool LastRunIsLablesVertical = true;
        public bool FastEditModeIsFixed = true;
        public bool IsAutoUpdateEnabled = false;
        public bool IsCheatsEnabled = false;
        public int FastEditValue = 30;
        public int TabItemSelected = 0;
        public string ProgramLanguage = "";
        public WindowState LastRunWindowState = WindowState.Normal;
        public Point LastRunLocation = new Point(0, 0);
        public Size LastRunSize = new Size(1000, 500);
    }

    public class Settings
    {
        public SettingsFields Fields;

        public Settings()
        {
            Fields = new SettingsFields();
        }

        public void WriteXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(SettingsFields));
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor");
            TextWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor//settings.xml");
            ser.Serialize(writer, Fields);
            writer.Close();

            Globals.Logger.Debug("Settings.xml has been written");
        }

        public void ReadXml()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor//settings.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(SettingsFields));
                TextReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor//settings.xml");
                Fields = ser.Deserialize(reader) as SettingsFields;
                reader.Close();

                Globals.Logger.Debug("Settings.xml has been loaded");
            }
            else
                Globals.Logger.Error("Settings.xml not found");
        }
    }
}
