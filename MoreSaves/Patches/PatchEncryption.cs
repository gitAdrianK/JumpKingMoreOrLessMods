namespace MoreSaves.Patches
{
    using System;
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems.Inventory;
    using JumpKing.MiscSystems.Achievements;
    using JumpKing.SaveThread;

    public static class PatchEncryption
    {
        private static readonly Action<string, CombinedSaveFile> DelegateSaveCombinedSaveFile;
        private static readonly Action<string, PlayerStats> DelegateSavePlayerStats;
        private static readonly Action<string, EventFlagsSave> DelegateSaveEventFlags;
        private static readonly Action<string, Inventory> DelegateSaveInventory;

        private static readonly Func<string, CombinedSaveFile> DelegateLoadCombinedSaveFile;
        private static readonly Func<string, PlayerStats> DelegateLoadPlayerStats;
        private static readonly Func<string, EventFlagsSave> DelegateLoadEventFlags;
        private static readonly Func<string, Inventory> DelegateLoadInventory;

        static PatchEncryption()
        {
            var typeEncryption = AccessTools.TypeByName("FileUtil.Encryption.Encryption");

            var saveFile = typeEncryption.GetMethod("SaveFile");
            DelegateSaveCombinedSaveFile = (Action<string, CombinedSaveFile>)saveFile
                .MakeGenericMethod(typeof(CombinedSaveFile))
                .CreateDelegate(typeof(Action<string, CombinedSaveFile>));
            DelegateSavePlayerStats = (Action<string, PlayerStats>)saveFile
                .MakeGenericMethod(typeof(PlayerStats))
                .CreateDelegate(typeof(Action<string, PlayerStats>));
            DelegateSaveEventFlags = (Action<string, EventFlagsSave>)saveFile
                .MakeGenericMethod(typeof(EventFlagsSave))
                .CreateDelegate(typeof(Action<string, EventFlagsSave>));
            DelegateSaveInventory = (Action<string, Inventory>)saveFile
                .MakeGenericMethod(typeof(Inventory))
                .CreateDelegate(typeof(Action<string, Inventory>));

            var loadFile = typeEncryption.GetMethod("LoadFile");
            DelegateLoadCombinedSaveFile = (Func<string, CombinedSaveFile>)loadFile
                .MakeGenericMethod(typeof(CombinedSaveFile))
                .CreateDelegate(typeof(Func<string, CombinedSaveFile>));
            DelegateLoadPlayerStats = (Func<string, PlayerStats>)loadFile
                .MakeGenericMethod(typeof(PlayerStats))
                .CreateDelegate(typeof(Func<string, PlayerStats>));
            DelegateLoadEventFlags = (Func<string, EventFlagsSave>)loadFile
                .MakeGenericMethod(typeof(EventFlagsSave))
                .CreateDelegate(typeof(Func<string, EventFlagsSave>));
            DelegateLoadInventory = (Func<string, Inventory>)loadFile
                .MakeGenericMethod(typeof(Inventory))
                .CreateDelegate(typeof(Func<string, Inventory>));
        }

        public static void SaveCombinedSaveFile(string path, CombinedSaveFile obj) =>
            DelegateSaveCombinedSaveFile(path, obj);

        public static void SavePlayerStats(string path, PlayerStats obj) =>
            DelegateSavePlayerStats(path, obj);

        public static void SaveEventFlags(string path, EventFlagsSave obj) =>
            DelegateSaveEventFlags(path, obj);

        public static void SaveInventory(string path, Inventory obj) =>
            DelegateSaveInventory(path, obj);

        public static CombinedSaveFile LoadCombinedSaveFile(string path) =>
            DelegateLoadCombinedSaveFile(path);

        public static PlayerStats LoadPlayerStats(string path) =>
            DelegateLoadPlayerStats(path);

        public static EventFlagsSave LoadEventFlags(string path) =>
            DelegateLoadEventFlags(path);

        public static Inventory LoadInventory(string path) =>
            DelegateLoadInventory(path);
    }
}
