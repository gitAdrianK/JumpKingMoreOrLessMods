namespace LessLocationText
{
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.Mods;
    using LessLocationText.Util;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.LessLocationText";
        private const string HARMONY_IDENTIFIER = IDENTIFIER + ".Harmony";
        private const string PREFERENCES_FILE = IDENTIFIER + ".Settings.xml";

        private static string PreferencesPath { get; set; }
        public static Preferences Preferences { get; private set; }

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
        }

        private static void SavePreferencesToFile(object sender, PropertyChangedEventArgs args)
            => Serialization.SaveToFile(Preferences, PreferencesPath);
    }
}
