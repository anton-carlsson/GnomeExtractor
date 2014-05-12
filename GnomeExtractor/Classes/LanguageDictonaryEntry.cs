using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GnomeExtractor
{
    public class LanguageDictonaryEntry
    {
        public string LanguageTitle { get; set; }
        public string About { get; set; }
        public string Attributes { get; set; }
        public string AutoUpdating { get; set; }
        public string Changes { get; set; }
        public string CheatMode { get; set; }
        public string Cheats { get; set; }
        public string Coal { get; set; }
        public string Squads { get; set; }
        public string Copper { get; set; }
        public string Emerald { get; set; }
        public string Exit { get; set; }
        public string Export { get; set; }
        public string FastRedactor { get; set; }
        public string File { get; set; }
        public string Gold { get; set; }
        public string Help { get; set; }
        public string Iron { get; set; }
        public string Lead { get; set; }
        public string Malachite { get; set; }
        public string Minerals { get; set; }
        public string Open { get; set; }
        public string OpenToolTip { get; set; }
        public string Other { get; set; }
        public string Platinum { get; set; }
        public string Professions { get; set; }
        public string ReadGnome { get; set; }
        public string Saphire { get; set; }
        public string Save { get; set; }
        public string SaveToolTip { get; set; }
        public string Settings { get; set; }
        public string Silver { get; set; }
        public string Skills { get; set; }
        public string Statistics { get; set; }
        public string Tin { get; set; }
        public string Updating { get; set; }
        public string Welcome { get; set; }
        public string World { get; set; }
        public string WorldName { get; set; }
        public string Discussion { get; set; }
        public string DiscussionLink { get; set; }
        public string License { get; set; }
        public string Project { get; set; }
        public string ProjectLink { get; set; }
        public string AboutTitle { get; set; }
        public string Title { get; set; }
        public string Cancel { get; set; }
        public string Combat { get; set; }
        public string FixedEditMode { get; set; }
        public string FastEditorTitle { get; set; }
        public string Labor { get; set; }
        public string RandomEditMode { get; set; }
        public string Add { get; set; }
        public string Remove { get; set; }
        public string ExportToolTip { get; set; }
        public string AnimalHusbandry { get; set; }
        public string Armor { get; set; }
        public string ArmorCrafting { get; set; }
        public string Axe { get; set; }
        public string Blacksmithing { get; set; }
        public string Bonecarving { get; set; }
        public string Brawling { get; set; }
        public string Brewing { get; set; }
        public string Butchery { get; set; }
        public string Caretaking { get; set; }
        public string Carpentry { get; set; }
        public string chosenFile { get; set; }
        public string Construction { get; set; }
        public string Cooking { get; set; }
        public string Crossbow { get; set; }
        public string Dodge { get; set; }
        public string Engineering { get; set; }
        public string errorFastEditValue { get; set; }
        public string errorGridEditName { get; set; }
        public string Farming { get; set; }
        public string Fighting { get; set; }
        public string GemCutting { get; set; }
        public string Gun { get; set; }
        public string Hammer { get; set; }
        public string Hauling { get; set; }
        public string Horticulture { get; set; }
        public string JewelryMaking { get; set; }
        public string Leatherworking { get; set; }
        public string Machining { get; set; }
        public string make65535 { get; set; }
        public string Masonry { get; set; }
        public string Mechanic { get; set; }
        public string Medic { get; set; }
        public string Metalworking { get; set; }
        public string Mining { get; set; }
        public string Pottery { get; set; }
        public string Prospecting { get; set; }
        public string saveDoneMessage { get; set; }
        public string Shield { get; set; }
        public string Smelting { get; set; }
        public string Stonecarving { get; set; }
        public string Sword { get; set; }
        public string Tailoring { get; set; }
        public string Tinkering { get; set; }
        public string tryWrongPath { get; set; }
        public string warn { get; set; }
        public string WeaponCrafting { get; set; }
        public string Weaving { get; set; }
        public string Woodcarving { get; set; }
        public string Woodcutting { get; set; }
        public string worldName { get; set; }
        public string Name { get; set; }
        public string Charm { get; set; }
        public string Curiosity { get; set; }
        public string Fitness { get; set; }
        public string Focus { get; set; }
        public string Nimbleness { get; set; }
        public string downloadNewVersion { get; set; }
        public string latestVersion { get; set; }
        public string newestVersion { get; set; }
        public string updateDialogCaption { get; set; }
        public string gameNotFound { get; set; }
        public string localizationNotFound { get; set; }
        public string loclibNotFound { get; set; }
        public string backupDoneMessage { get; set; }
        public string exportDoneMessage { get; set; }
        public string connectionError { get; set; }
        public string Profession { get; set; }
        public string professionAlreadyExists { get; set; }
        public string LanguageMenu { get; set; }

        public LanguageDictonaryEntry()
        {
            this.LanguageTitle = "English";
            this.About = "_About...";
            this.Attributes = "Attributes";
            this.AutoUpdating = "Auto checking updates";
            this.Changes = "Changes";
            this.CheatMode = "Cheat _mode";
            this.Cheats = "_Cheats";
            this.Coal = "Coal:";
            this.Squads = "Squads";
            this.Copper = "Copper:";
            this.Emerald = "Emerald:";
            this.Exit = "_Exit";
            this.Export = "E_xport to a CSV format";
            this.FastRedactor = "_Fast editor...";
            this.File = "_File";
            this.Gold = "Gold:";
            this.Help = "_Help";
            this.Iron = "Iron:";
            this.Lead = "Lead:";
            this.Malachite = "Malachite:";
            this.Minerals = "Minerals";
            this.Open = "_Open...";
            this.OpenToolTip = "Open";
            this.Other = "Other";
            this.Platinum = "Platinum:";
            this.Professions = "Professions";
            this.ReadGnome = "Read gnome";
            this.Saphire = "Saphire:";
            this.Save = "_Save changes...";
            this.SaveToolTip = "Save changes";
            this.Settings = "_Settings";
            this.Silver = "Silver:";
            this.Skills = "Skills";
            this.Statistics = "Statistics";
            this.Tin = "Tin:";
            this.Updating = "Check for updates...";
            this.Welcome = "Welcome to the Gnome Extractor. Open Gnomoria save for continue.";
            this.World = "World";
            this.WorldName = "Name:";
            this.Discussion = "Discussion page:";
            this.DiscussionLink = "http://forums.gnomoria.com/index.php?topic=1733";
            this.License = "This program is free software; You can redistribute it and/or modify, it under" +
            "the termsof the GNU General Public License aspublished by the Free Software " +
            "Foundation; either version 2 of the License, or (at your option) any later version.";
            this.Project = "Website:";
            this.ProjectLink = "http://gnomex.tk";
            this.AboutTitle = "About Gnome Extractor";
            this.Cancel = "Cancel";
            this.Combat = "Combat";
            this.FixedEditMode = "Same";
            this.FastEditorTitle = "Select a value";
            this.Labor = "Labor";
            this.RandomEditMode = "Random";
            this.Add = "Add";
            this.Remove = "Remove";
            this.ExportToolTip = "Export to CSV format";
            this.AnimalHusbandry = "AnimalHusbandry";
            this.Armor = "Armor";
            this.ArmorCrafting = "ArmorCrafting";
            this.Axe = "Axe";
            this.Blacksmithing = "Blacksmithing";
            this.Bonecarving = "Bonecarving";
            this.Brawling = "Brawling";
            this.Brewing = "Brewing";
            this.Butchery = "Butchery";
            this.Caretaking = "Caretaking";
            this.Carpentry = "Carpentry";
            this.chosenFile = "File selected:";
            this.Construction = "Construction";
            this.Cooking = "Cooking";
            this.Crossbow = "Crossbow";
            this.Dodge = "Dodge";
            this.Engineering = "Engineering";
            this.errorFastEditValue = "Only integers from  1 to 50000 are accepted";
            this.errorGridEditName = "The Gnome's name may not exceed 24 characters";
            this.Farming = "Farming";
            this.Fighting = "Fighting";
            this.GemCutting = "Gem Cutting";
            this.Gun = "Gun";
            this.Hammer = "Hammer";
            this.Hauling = "Hauling";
            this.Horticulture = "Horticulture";
            this.JewelryMaking = "Jewelry Making";
            this.Leatherworking = "Leatherworking";
            this.Machining = "Machining";
            this.make65535 = "This action cannot be undone. Use on your own risk!";
            this.Masonry = "Masonry";
            this.Mechanic = "Mechanic";
            this.Medic = "Medic";
            this.Metalworking = "Metalworking";
            this.Mining = "Mining";
            this.Pottery = "Pottery";
            this.Prospecting = "Prospecting";
            this.saveDoneMessage = "All changes are saved to the source file:";
            this.Shield = "Shield";
            this.Smelting = "Smelting";
            this.Stonecarving = "Stonecarving";
            this.Sword = "Sword";
            this.Tailoring = "Tailoring";
            this.Tinkering = "Tinkering";
            this.tryWrongPath = "You are trying to open a file from a wrong directory.";
            this.warn = "Warning";
            this.WeaponCrafting = "Weapon Crafting";
            this.Weaving = "Weaving";
            this.Woodcarving = "Woodcarving";
            this.Woodcutting = "Woodcutting";
            this.worldName = "World name:";
            this.Name = "Name";
            this.Charm = "Charm";
            this.Curiosity = "Curiosity";
            this.Fitness = "Fitness";
            this.Focus = "Focus";
            this.Nimbleness = "Nimbleness";
            this.downloadNewVersion = "go to the official website to download?";
            this.latestVersion = "No updates avaiable at this time, you have lastest version";
            this.newestVersion = "Found a more recent version:";
            this.updateDialogCaption = "Update";
            this.gameNotFound = "File Gnomoria.exe not found, please install the program in game folder";
            this.localizationNotFound = "Localization file not found for that language";
            this.loclibNotFound = "File loclib.dll not found, please reinstall the program";
            this.backupDoneMessage = "backup saved to:";
            this.exportDoneMessage = "Export tables to CSV completed successfully. Please, check Export folder";
            this.connectionError = "Connection error";
            this.Profession = "Profession";
            this.professionAlreadyExists = "Profession with same title already exists";
            this.LanguageMenu = "Language";
        }
    }
}
