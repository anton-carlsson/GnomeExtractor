using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Game;
using GameLibrary;

namespace GnomeExtractor
{
    public class Gnome : INotifyPropertyChanged
    {
        string name;
        int id;
        int level;
        int row;
        int column;
        int position;
        Profession profession;
        int mining;
        List<SkillEntry> laborSkills = new List<SkillEntry>();
        List<SkillEntry> combatSkills = new List<SkillEntry>();
        List<AttributeEntry> attributes = new List<AttributeEntry>();

        public Gnome(Character gnome, int level, int row, int column, int mapCellPosition, int realIndex)
        {
            this.name = gnome.Name();
            this.id = realIndex;
            this.level = level;
            this.row = row;
            this.column = column;
            this.position = mapCellPosition;
            this.profession = gnome.Mind.Profession;
            this.mining = gnome.SkillLevel(CharacterSkillType.Mining);
            foreach (var skill in SkillDef.AllLaborSkills())
                this.laborSkills.Add(new SkillEntry(skill, gnome.SkillLevel(skill), gnome.Mind.IsSkillAllowed(skill)));
            foreach (var skill in SkillDef.AllCombatSkills())
                this.combatSkills.Add(new SkillEntry(skill, gnome.SkillLevel(skill), gnome.Mind.IsSkillAllowed(skill)));
            foreach (var attribute in Enum.GetValues(typeof(CharacterAttributeType)))
                this.attributes.Add(new AttributeEntry((CharacterAttributeType)attribute, gnome.AttributeLevel((CharacterAttributeType)attribute)));
        }

        public int Level
        { get { return this.level; } }

        public int Row
        { get { return this.row; } }

        public int Column
        { get { return this.column; } }

        public int Position
        { get { return this.position; } }

        public int ID
        { get { return this.id; } }

        public string Name
        { get { return this.name; } set { this.name = value; OnPropertyChanged("Name"); } }

        public Profession Profession
        { get { return this.profession; } set { this.profession = value; OnPropertyChanged("Profession"); } }

        public int Mining
        { get { return this.mining; } set { this.mining = value; OnPropertyChanged("Mining"); } }

        public List<SkillEntry> LaborSkills
        { get { return this.laborSkills; } }

        public List<SkillEntry> CombatSkills
        { get { return this.combatSkills; } }

        public List<AttributeEntry> Attributes
        { get { return this.attributes; } }

        public void SetAllowedSkills(SkillGroup allowedSkills)
        {
            profession.AllowedSkills = allowedSkills;
            foreach (var skill in laborSkills)
                skill.IsAllowed = allowedSkills.IsSkillAllowed(skill.Type);
        }

        /// <summary>
        /// Updating AllowedSkills property for custom professions
        /// </summary>
        public void UpdateAllowedSkills()
        {
            if (profession.Title == "Custom")
            {
                profession = new Profession(profession.Title);
                profession.AllowedSkills.ClearAll();
                foreach (var skill in laborSkills)
                    if (skill.IsAllowed) profession.AllowedSkills.AddSkill(skill.Type);
            }
        }

        public List<SkillEntry> GetClonedSkills()
        {
            List<SkillEntry> skills = new List<SkillEntry>();
            foreach (var skill in laborSkills)
                skills.Add(new SkillEntry(skill.Type, skill.Level, skill.IsAllowed));
            return skills;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }

    /// <summary>
    /// Gnome attribute entry
    /// </summary>
    public class AttributeEntry : INotifyPropertyChanged
    {
        float level;
        string name;
        CharacterAttributeType type;

        public AttributeEntry(CharacterAttributeType type, float level)
        {
            this.level = level;
            this.name = type.ToString();
            this.type = type;
        }

        public float Level
        { get { return this.level * 100; } set { this.level = value / 100; OnPropertyChanged("Level"); } }

        public string Name
        { get { return this.name; } set { this.name = value; OnPropertyChanged("Name"); } }

        public CharacterAttributeType Type
        { get { return this.type; } }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Labor or combat skill entry
    /// </summary>
    public class SkillEntry : INotifyPropertyChanged
    {
        int level;
        string name;
        CharacterSkillType type;
        bool isAllowed;

        public SkillEntry(CharacterSkillType type, int level, bool isAllowed)
        {
            this.level = level;
            this.name = type.ToString();
            if (type == CharacterSkillType.LaborEnd) this.name = "Hauling";
            this.type = type;
            this.isAllowed = isAllowed;
        }

        public int Level
        { get { return this.level; } set { this.level = value; OnPropertyChanged("Level"); } }

        public string Name
        { get { return this.name; } set { this.name = value; OnPropertyChanged("Name"); } }

        public CharacterSkillType Type
        { get { return this.type; } }

        public bool IsAllowed
        { get { return this.isAllowed; } set { this.isAllowed = value; OnPropertyChanged("IsAllowed"); } }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
