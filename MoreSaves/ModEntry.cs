namespace MoreSaves
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using JumpKing.SaveThread;
    using LanguageJK;
    using Menus;
    using Menus.Models;
    using Menus.Nodes;
    using Microsoft.Xna.Framework;
    using Patches;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreSaves";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

        private const string Auto = ModStrings.Auto;
        private const string Manual = ModStrings.Manual;
        private const string SavesPerma = ModStrings.SavesPerma;

        public static string DllDirectory { get; private set; }
        //public static string ExeDirectory { get; private set; }

        public static string SaveName { get; set; }

        [MainMenuItemSetting]
        [UsedImplicitly]
        public static TextButton LoadAutoSavefile(object factory, GuiFormat format)
        {
            ModelLoadOptions.SetupButtons();
            return new TextButton("Load Automatic Save",
                ModelLoadOptions.CreateLoadOptions(factory, format, 0, ModelLoadOptions.PageOption.Auto));
        }

        [MainMenuItemSetting]
        [UsedImplicitly]
        public static TextButton LoadManualSavefile(object factory, GuiFormat format)
        {
            ModelLoadOptions.SetupButtons();
            return new TextButton("Load Manual Save",
                ModelLoadOptions.CreateLoadOptions(factory, format, 0, ModelLoadOptions.PageOption.Manual));
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TextButton CreateManualSave(object factory, GuiFormat format)
            => new TextButton("Create Manual Save", new NodeCreateManualSave());

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ButtonTextExplorer OpenFolderExplorer(object factory, GuiFormat format)
            => new ButtonTextExplorer("Open Saves Folder", new NodeOpenFolderExplorer(), Color.Lime);

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        [UsedImplicitly]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HarmonyIdentifier);
#if DEBUG
            Debugger.Launch();
#endif
            _ = new PatchSaveHelper(harmony);
            _ = new PatchSaveLube(harmony);

            DllDirectory =
                $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
            //ExeDirectory =
            //    $"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}{Path.DirectorySeparatorChar}";

            if (!Directory.Exists($"{DllDirectory}{Manual}"))
            {
                _ = Directory.CreateDirectory($"{DllDirectory}{Manual}");
            }

            if (!Directory.Exists($"{DllDirectory}{Auto}"))
            {
                _ = Directory.CreateDirectory($"{DllDirectory}{Auto}");
            }

            ModelLoadOptions.SetupButtons();

            SaveName = string.Empty;
        }

        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        [UsedImplicitly]
        public static void OnLevelStart()
        {
            if (LevelDebugState.instance != null)
            {
                return;
            }

            SaveName = SanitizeName(GetSaveName());

            PatchXmlWrapper.Serialize(PatchSaveLube.GetGeneralSettings(), Auto, SaveName, SavesPerma);
            PatchEncryption.SaveInventory(PatchInventoryManager.GetInventory(), Auto, SaveName, SavesPerma);
            PatchEncryption.SaveEventFlags(EventFlagsSave.Save, Auto, SaveName, SavesPerma);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPlayerStats(), ModStrings.Stats, Auto, SaveName,
                SavesPerma);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPermaStats(), ModStrings.Permanent, Auto,
                SaveName, SavesPerma);
        }

        /// <summary>
        ///     Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        [UsedImplicitly]
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

            return EventFlagsSave.ContainsFlag(StoryEventFlags.StartedGhost)
                ? language.GAMETITLESCREEN_GHOST_OF_THE_BABE
                : language.GAMETITLESCREEN_NEW_GAME;
        }

        private static string SanitizeName(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
            {
                name = "Save_emptyName";
            }

            name = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '#'));

            name = Path.GetInvalidPathChars().Aggregate(name, (current, c) => current.Replace(c, '#'));

            name = Regex.Replace(name, @"^\.\.$", ". .");
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
