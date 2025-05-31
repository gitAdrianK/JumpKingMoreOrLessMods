namespace MoreTextOptions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using Microsoft.Xna.Framework;
    using MoreTextOptions.Util;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.MoreTextOptions";
        private const string HARMONY_IDENTIFIER = IDENTIFIER + ".Harmony";
        private const string PREFERENCES_FILE = IDENTIFIER + ".Settings.xml";

        public static readonly Regex REGEX = new Regex("{color=\"(#(?:[0-9a-fA-F]{2}){3})\"}", RegexOptions.IgnoreCase);
        private static string PreferencesPath { get; set; }
        public static Preferences Preferences { get; private set; }
        public static int OffsetX { get; private set; }
        public static int OffsetY { get; private set; }

        [MainMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static TextInfo HintOptionsLocation(object factory, GuiFormat format)
            => new TextInfo("Options in the pausemenu!", Color.Lime);

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
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            PreferencesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PREFERENCES_FILE);
            Preferences = Serialization.ReadFromFile<Preferences>(PreferencesPath);
            Preferences.PropertyChanged += SavePreferencesToFile;

            var spriteFont = Game1.instance.contentManager.font.MenuFont;
            var red = spriteFont.MeasureString("Red").ToPoint();
            var green = spriteFont.MeasureString("Green").ToPoint();
            var blue = spriteFont.MeasureString("Blue").ToPoint();
            OffsetX = Math.Max(red.X, Math.Max(green.X, blue.X));
            OffsetY = red.Y;
        }

        private static void SavePreferencesToFile(object sender, PropertyChangedEventArgs args)
            => Serialization.SaveToFile(Preferences, PreferencesPath);
    }
}
