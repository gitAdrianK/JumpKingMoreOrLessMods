namespace MoreSaves
{
    using System.IO;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using Menus;
    using Menus.Models;
    using Menus.Nodes;
    using Microsoft.Xna.Framework;
    using Util;
    using SaveManager = Saves.SaveManager;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreSaves";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

        public static string DllDirectory { get; private set; }
        public static SaveManager SaveManager { get; private set; }

        [MainMenuItemSetting]
        [UsedImplicitly]
        public static TextButton LoadAutoSavefile(object factory, GuiFormat format)
        {
            ModelLoadOptions.SetupButtons();
            return new TextButton("Load Automatic Save",
                ModelLoadOptions.CreateLoadOptions(factory, format, 0, SaveType.Auto));
        }

        [MainMenuItemSetting]
        [UsedImplicitly]
        public static TextButton LoadManualSavefile(object factory, GuiFormat format)
        {
            ModelLoadOptions.SetupButtons();
            return new TextButton("Load Manual Save",
                ModelLoadOptions.CreateLoadOptions(factory, format, 0, SaveType.Manual));
        }

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static TextButton CreateManualSave(object factory, GuiFormat format)
            => new TextButton("Create Manual Save", new NodeCreateManualSave());

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
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
#if DEBUG
            Debugger.Launch();
#endif
            var harmony = new Harmony(HarmonyIdentifier);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            DllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            SaveManager = new SaveManager(DllDirectory);

            ModelLoadOptions.SetupButtons();
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

            SaveManager.StartSaving();
            SaveManager.SaveAllAuto();
        }

        /// <summary>
        ///     Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        [UsedImplicitly]
        public static void OnLevelEnd()
        {
            // Save settings and stats here an additional time as they get messed up otherwise.
            SaveManager.SaveGeneralSettings();
            SaveManager.SavePermaPlayerStats();
            SaveManager.StopSaving();
        }
    }
}
