using System.Collections.Generic;
using Game;
using GameLibrary;
using System;

namespace GnomeExtractor
{
    public class Statistics
    {
        string worldName;
        MineralStatistic[] minerals = new MineralStatistic[Enum.GetNames(typeof(MineralID)).Length];

        public Statistics(GnomanEmpire gnomanEmpire)
        {
            Globals.Logger.Info("Creating statistics...");
            if (gnomanEmpire == null)
            {
                Globals.Logger.Error("World is not loaded");
                return;
            }

            worldName = gnomanEmpire.World.AIDirector.PlayerFaction.Name;

            var names = Enum.GetNames(typeof(MineralID));
            for (int i = 0; i < names.Length; i++)
                minerals[i] = new MineralStatistic((MineralID)Enum.Parse(typeof(MineralID), names[i]));

            for (var level = 0; level < gnomanEmpire.Map.MapDepth; level++)
                for (var height = 0; height < gnomanEmpire.Map.MapHeight; height++)
                    for (var width = 0; width < gnomanEmpire.Map.MapWidth; width++)
                    {
                        var cell = gnomanEmpire.Map.GetCell(level, height, width);
                        if (!cell.HasEmbeddedWall()) continue;
                        var embeddedWall = cell.EmbeddedWall;
                        var mineral = embeddedWall as Mineral;
                        if (mineral == null) continue;

                        int index2 = 0;
                        foreach (string i in Enum.GetNames(typeof(MineralID)))
                        {
                            if (mineral.MaterialID == ((MineralID)Enum.Parse(typeof(MineralID), i)).GetHashCode())
                                minerals[index2].Count++;
                            index2++;
                        }
                    }

            Globals.Logger.Info("Statistics has been created");
        }

        public MineralStatistic[] Minerals
        { get { return minerals; } }

        public string WorldName
        { get { return worldName; } }

        /// <summary>
        /// Element of statistics
        /// </summary>
        public class MineralStatistic
        {
            MineralID id;
            int count = 0;

            public MineralStatistic(MineralID id)
            { this.id = id; }

            /// <summary>
            /// Returns minerals name
            /// </summary>
            public string Name
            { get { return id.ToString(); } }

            /// <summary>
            /// Returns minerals count
            /// </summary>
            public int Count
            { get { return count; } set { count = value; } }
        }
    }

    public enum MineralID
    {
        Copper = 0x13, Tin = 0x15, Malachite = 20, Iron = 0x17,
        Lead = 0x19, Silver = 0x1a, Gold = 0x1b, Platinum = 0x1d,
        Coal = 0x12, Saphire = 0x1f, Emerald = 30
    }
}
