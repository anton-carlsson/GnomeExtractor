using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace GnomeExtractor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Collecting information
            if (File.Exists("Gnomoria.exe")) Globals.Logger.Debug("Gnomoria version found: {0}", FileVersionInfo.GetVersionInfo("Gnomoria.exe").FileVersion);
            else
            {
                Globals.Logger.Fatal("Gnomoria.exe is not found");
                MessageBox.Show("Gnomoria.exe is not found, please install program in the game folder");
                Environment.Exit(0);
            }
            Globals.Logger.Debug("Gnome Extractor version: {0}", Globals.ProgramVersion);
            Globals.Logger.Debug("OS found: {0}", Environment.OSVersion);
            Globals.Logger.Debug("OS found: {0}", (Environment.Is64BitOperatingSystem) ? "64 bit" : "32 bit");
            Globals.Logger.Debug(".Net version found: {0}", Environment.Version);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Console.Write("\nPress enter to exit...");
            Console.ReadLine();
            base.OnExit(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //MessageBox.Show(e.ExceptionObject.ToString());
            Globals.Logger.Fatal(e.ExceptionObject.ToString());
            //Process.Start("notepad.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor//output.log");
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "ru")
                MessageBox.Show("Обнаружена ошибка. Пожалуйста, отправьте содержимое файла output.log в обсуждение группы http://vk.com/gnomoria");
            else
                MessageBox.Show("An error has occured. Please, send your output.log file to forum discussion");
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor//");
        }
    }
}
