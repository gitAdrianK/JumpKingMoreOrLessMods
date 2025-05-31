namespace MoreSaves
{
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.SaveThread;
    using LanguageJK;
    using MoreSaves.Menues.Models;
    using MoreSaves.Patches;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.MoreSaves";
        private const string HARMONY_IDENTIFIER = IDENTIFIER + ".Harmony";

        private const string AUTO = ModStrings.AUTO;
        private const string MANUAL = ModStrings.MANUAL;
        private const string SAVES_PERMA = ModStrings.SAVES_PERMA;

        public static string DllDirectory { get; private set; }
        public static string ExeDirectory { get; private set; }

        public static string SaveName { get; set; }

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HARMONY_IDENTIFIER);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif
            _ = new PatchSaveHelper(harmony);
            _ = new PatchSaveLube(harmony);

            DllDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            ExeDirectory = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}{Path.DirectorySeparatorChar}";

            if (!Directory.Exists($"{DllDirectory}{MANUAL}"))
            {
                _ = Directory.CreateDirectory($"{DllDirectory}{MANUAL}");
            }
            if (!Directory.Exists($"{DllDirectory}{AUTO}"))
            {
                _ = Directory.CreateDirectory($"{DllDirectory}{AUTO}");
            }

            ModelLoadOptions.SetupButtons();

            SaveName = string.Empty;
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            if (LevelDebugState.instance != null)
            {
                return;
            }

            SaveName = SanitizeName(GetSaveName());

            PatchXmlWrapper.Serialize(PatchSaveLube.GetGeneralSettings(), AUTO, SaveName, SAVES_PERMA);
            PatchEncryption.SaveInventory(PatchInventoryManager.GetInventory(), AUTO, SaveName, SAVES_PERMA);
            PatchEncryption.SaveEventFlags(EventFlagsSave.Save, AUTO, SaveName, SAVES_PERMA);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPlayerStats(), ModStrings.STATS, AUTO, SaveName, SAVES_PERMA);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPermaStats(), ModStrings.PERMANENT, AUTO, SaveName, SAVES_PERMA);
        }

        /// <summary>
        /// Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd() => SaveName = string.Empty;

        private static string GetSaveName()
        {
            var contentManager = Game1.instance.contentManager;
            // This should never happen
            if (contentManager == null)
            {
                return "Debug";
            }

            // Not a vanilla map
            if (contentManager.level != null)
            {
                return contentManager.level.Name;
            }

            // Which vanilla map
            if (EventFlagsSave.ContainsFlag(StoryEventFlags.StartedNBP))
            {
                return language.GAMETITLESCREEN_NEW_BABE_PLUS;
            }
            if (EventFlagsSave.ContainsFlag(StoryEventFlags.StartedGhost))
            {
                return language.GAMETITLESCREEN_GHOST_OF_THE_BABE;
            }
            return language.GAMETITLESCREEN_NEW_GAME;
        }

        private static string SanitizeName(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
            {
                name = "Save_emptyName";
            }
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '#');
            }
            foreach (var c in Path.GetInvalidPathChars())
            {
                name = name.Replace(c, '#');
            }
            name = Regex.Replace(name, "^\\.\\.$", ". .");
            name = Regex.Replace(name, "^[c|C][o|O][n|N]$", $"Save_{name}");
            name = Regex.Replace(name, "^[p|P][r|R][n|N]$", $"Save_{name}");
            name = Regex.Replace(name, "^[a|A][u|U][x|X]$", $"Save_{name}");
            name = Regex.Replace(name, "^[n|N][u|U][l|L]$", $"Save_{name}");
            name = Regex.Replace(name, "^[c|C][o|O][m|M]\\d$", $"Save_{name}");
            name = Regex.Replace(name, "^[l|L][p|P][t|T]\\d$", $"Save_{name}");

            return name;
        }
    }
}
