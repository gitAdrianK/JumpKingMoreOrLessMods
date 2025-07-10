namespace LessLocationText
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using Util;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.LessLocationText";
        private const string HarmonyIdentifier = Identifier + ".Harmony";
        private const string PreferencesFile = Identifier + ".Settings.xml";

        private static string PreferencesPath { get; set; }
        public static Preferences Preferences { get; private set; }

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
        }

        private static void SavePreferencesToFile(object sender, PropertyChangedEventArgs args)
            => Serialization.SaveToFile(Preferences, PreferencesPath);
    }
}
