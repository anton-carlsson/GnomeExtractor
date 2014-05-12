using System.Collections.Generic;
using System.ComponentModel;
using Game;

namespace GnomeExtractor
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        bool isCheatsEnabled;
        Statistics statistics;
        LanguageDictonaryEntry language;
        List<Gnome> gnomes = new List<Gnome>();
        List<Profession> professions = new List<Profession>();
        List<SkillEntry> skills = new List<SkillEntry>();
        List<LanguageDictonaryEntry> languages = new List<LanguageDictonaryEntry>();

        public List<Gnome> Gnomes
        { get { return this.gnomes; } set { this.gnomes = value; OnPropertyChanged("Gnomes"); } }

        public List<Profession> Professions
        { get { return this.professions; } set { this.professions = value; OnPropertyChanged("Professions"); } }

        public List<SkillEntry> Skills
        { get { return this.skills; } set { this.skills = value; OnPropertyChanged("Skills"); } }

        public bool IsCheatsEnabled
        { get { return this.isCheatsEnabled; } set { this.isCheatsEnabled = value; OnPropertyChanged("IsCheatsEnabled"); } }

        public LanguageDictonaryEntry Language
        { get { return this.language; } set { this.language = value; OnPropertyChanged("Language"); } }

        public Statistics Statistics
        { get { return this.statistics; } set { this.statistics = value; OnPropertyChanged("Statistics"); } }

        public List<LanguageDictonaryEntry> Languages
        { get { return this.languages; } set { this.languages = value; OnPropertyChanged("Languages"); } }

        public void Clean()
        {
            professions = new List<Profession>();
            gnomes = new List<Gnome>();
            statistics = null;
            OnPropertyChanged("Professions");
            OnPropertyChanged("Gnomes");
            OnPropertyChanged("Statistics");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
