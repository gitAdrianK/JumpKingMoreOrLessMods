namespace MoreTextOptions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreTextOptions";
        private const string HarmonyIdentifier = Identifier + ".Harmony";
        private const string PreferencesFile = Identifier + ".Settings.xml";

        public static readonly Regex Regex = new Regex(
            @"\{(?:color=""#(?:[0-9a-fA-F]{2}){3}""\s+font=""[^""]+""|font=""[^""]+""\s+color=""#(?:[0-9a-fA-F]{2}){3}""|color=""#(?:[0-9a-fA-F]{2}){3}""|font=""[^""]+"")\}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static readonly Regex RegexFont = new Regex(
            @"font=""(?<Font>[^""]+)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static readonly Regex RegexColor = new Regex(
            @"color=""#(?<R>[0-9a-fA-F]{2})(?<G>[0-9a-fA-F]{2})(?<B>[0-9a-fA-F]{2})""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string PreferencesPath { get; set; }
        public static Preferences Preferences { get; private set; }

        public static int OffsetX { get; private set; }
        public static int OffsetY { get; private set; }

        public static Dictionary<string, string> Translations { get; set; }

        [MainMenuItemSetting]
        [UsedImplicitly]
        public static TextInfo HintOptionsLocation(object factory, GuiFormat format)
            => new TextInfo("Options in the pause menu!", Color.Lime);

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static TextInfoExample DisplayExampleText(object factory, GuiFormat format)
            => new TextInfoExample();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static ToggleCustomText CustomText(object factory, GuiFormat format)
            => new ToggleCustomText();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderTextRed TextRed(object factory, GuiFormat format)
            => new SliderTextRed();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderTextGreen TextGreen(object factory, GuiFormat format)
            => new SliderTextGreen();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderTextBlue TextBlue(object factory, GuiFormat format)
            => new SliderTextBlue();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static ToggleDisableOutline ToggleOutline(object factory, GuiFormat format)
            => new ToggleDisableOutline();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static ToggleCustomOutline CustomOutline(object factory, GuiFormat format)
            => new ToggleCustomOutline();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderOutlineRed OutlineRed(object factory, GuiFormat format)
            => new SliderOutlineRed();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderOutlineGreen OutlineGreen(object factory, GuiFormat format)
            => new SliderOutlineGreen();

        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static SliderOutlineBlue OutlineBlue(object factory, GuiFormat format)
            => new SliderOutlineBlue();

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
              Debugger.Launch();
#endif
            var assembly = Assembly.GetExecutingAssembly();

            new Harmony(HarmonyIdentifier).PatchAll(assembly);

            PreferencesPath = Path.Combine(
                Path.GetDirectoryName(assembly.Location) ??
                throw new InvalidOperationException(),
                PreferencesFile);
            try
            {
                var prefs = XmlSerializerHelper.Deserialize<Preferences>(PreferencesPath);
                Preferences = prefs;
            }
            catch
            {
                Preferences = new Preferences();
                XmlSerializerHelper.Serialize(PreferencesPath, Preferences);
            }

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
        public static void OnLevelStart()
        {
            var level = Game1.instance.contentManager?.level;
            if (level == null)
            {
                return;
            }

            var root = level.Root;
            PatchSpriteBatch.LoadCustomFonts(root);

            var tags = level.Info.Tags;
            PatchStatsScreen.LoadEndingNames(tags);
            PatchPlayerStandStill.CheckStandStillRequirement(tags);
        }

        private static void SavePreferencesToFile(object sender, PropertyChangedEventArgs args)
            => XmlSerializerHelper.Serialize(PreferencesPath, Preferences);
    }
}
