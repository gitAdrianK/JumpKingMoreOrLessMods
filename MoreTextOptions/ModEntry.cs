namespace MoreTextOptions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using Menus;
    using Microsoft.Xna.Framework;
    using Patches;
    using Util;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreTextOptions";
        private const string HarmonyIdentifier = Identifier + ".Harmony";
        private const string PreferencesFile = Identifier + ".Settings.xml";

        public static readonly Regex Regex = new Regex("{color=\"(#(?:[0-9a-fA-F]{2}){3})\"}", RegexOptions.IgnoreCase);
        private static string PreferencesPath { get; set; }
        public static Preferences Preferences { get; private set; }
        public static int OffsetX { get; private set; }
        public static int OffsetY { get; private set; }

        [MainMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TextInfo HintOptionsLocation(object factory, GuiFormat format)
            => new TextInfo("Options in the pause menu!", Color.Lime);

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TextInfoExample DisplayExampleText(object factory, GuiFormat format)
            => new TextInfoExample();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleCustomText CustomText(object factory, GuiFormat format)
            => new ToggleCustomText();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderTextRed TextRed(object factory, GuiFormat format)
            => new SliderTextRed();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderTextGreen TextGreen(object factory, GuiFormat format)
            => new SliderTextGreen();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderTextBlue TextBlue(object factory, GuiFormat format)
            => new SliderTextBlue();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleDisableOutline ToggleOutline(object factory, GuiFormat format)
            => new ToggleDisableOutline();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleCustomOutline CustomOutline(object factory, GuiFormat format)
            => new ToggleCustomOutline();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderOutlineRed OutlineRed(object factory, GuiFormat format)
            => new SliderOutlineRed();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderOutlineGreen OutlineGreen(object factory, GuiFormat format)
            => new SliderOutlineGreen();

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderOutlineBlue OutlineBlue(object factory, GuiFormat format)
            => new SliderOutlineBlue();

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
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            PreferencesPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                PreferencesFile);
            Preferences = Serialization.ReadFromFile<Preferences>(PreferencesPath);
            Preferences.PropertyChanged += SavePreferencesToFile;

            var spriteFont = Game1.instance.contentManager.font.MenuFont;
            var red = spriteFont.MeasureString("Red").ToPoint();
            var green = spriteFont.MeasureString("Green").ToPoint();
            var blue = spriteFont.MeasureString("Blue").ToPoint();
            OffsetX = Math.Max(red.X, Math.Max(green.X, blue.X));
            OffsetY = red.Y;
        }

        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        [UsedImplicitly]
        public static void OnLevelStart()
        {
            PatchOldManEntity.LoadAndAssignFonts(Game1.instance.contentManager);
            PatchStatsScreen.LoadEndingNames();
        }

        private static void SavePreferencesToFile(object sender, PropertyChangedEventArgs args)
            => Serialization.SaveToFile(Preferences, PreferencesPath);
    }
}
