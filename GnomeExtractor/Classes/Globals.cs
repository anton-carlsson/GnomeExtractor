using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using GameLibrary;
using NLog;

namespace GnomeExtractor
{
    public class Globals
    {
        static int firstColumnNames = 3;
        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Gnome Extractor";
        static string appStartupPath = System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location);
        static Version programVersion = new Version("0.4.4");
        static Logger logger = LogManager.GetLogger("SampleLogger");
        static ConnectionViewModel viewModel = new ConnectionViewModel();
        
        /// <summary>
        /// Initialize global variables
        /// </summary>
        public static void Initialize()
        {
            Globals.Logger.Debug("Global variables initialization...");

            //foreach (var skill in SkillDef.AllLaborSkills())
            //    viewModel.Skills.Add(new SkillEntry(skill, 0, false));

            Globals.Logger.Debug("Global variables initialized");
        }

        #region Static properties
        public static int FirstColumnCount { get { return firstColumnNames; } }

        public static string AppDataPath { get { return appDataPath; } }

        public static string ApplicationFolder { get { return appStartupPath; } }

        public static Version ProgramVersion { get { return programVersion; } }

        public static Logger Logger { get { return logger; } }

        public static ConnectionViewModel ViewModel { get { return viewModel; } }
        #endregion

        #region Static methods
        public static void LoadLanguages()
        {
            Directory.CreateDirectory(Globals.ApplicationFolder + "//Languages");
            DirectoryInfo dir = new DirectoryInfo(Globals.ApplicationFolder + "//Languages");

            Globals.ViewModel.Languages.Add(new LanguageDictonaryEntry());

            FileInfo[] fi = dir.GetFiles("*.xml");

            foreach (var file in fi)
            {
                XmlSerializer ser = new XmlSerializer(typeof(LanguageDictonaryEntry));
                TextReader reader = new StreamReader(file.FullName);
                var language = ser.Deserialize(reader) as LanguageDictonaryEntry;
                if (language.LanguageTitle != "English")
                    Globals.ViewModel.Languages.Add(language);
                reader.Close();

                Globals.Logger.Debug("Language file " + file.Name + " has been loaded");
            }
        }

        public static void WriteEnglishXml()
        {
            LanguageDictonaryEntry language = new LanguageDictonaryEntry();

            XmlSerializer ser = new XmlSerializer(typeof(LanguageDictonaryEntry));
            Directory.GetParent(Directory.GetParent(Globals.ApplicationFolder).FullName);
            var path = Directory.GetParent(Directory.GetParent(Globals.ApplicationFolder).FullName).FullName + "//Languages//";
            Directory.CreateDirectory(path);
            TextWriter writer = new StreamWriter(path + "english.xml");
            ser.Serialize(writer, language);
            writer.Close();

            Globals.Logger.Debug("Settings.xml has been written");
        }
        #endregion
    }
}
