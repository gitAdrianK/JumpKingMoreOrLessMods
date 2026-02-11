namespace MoreSaves.Patches
{
    using System;
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems.Inventory;
    using JumpKing.MiscSystems.Achievements;
    using JumpKing.SaveThread;
    using Util;

    public static class PatchSaveLube
    {
        private static readonly AccessTools.FieldRef<CombinedSaveFile> CombinedSaveRef;

        private static readonly Func<EventFlagsSave> GetEventFlags;
        private static readonly Action<EventFlagsSave> SetEventFlags;

        private static readonly Func<PlayerStats> GetPlayerStatsAttemptSnapshot;
        private static readonly Action<PlayerStats> SetPlayerStatsAttemptSnapshot;

        private static readonly Func<PlayerStats> GetPermanentPlayerStats;
        private static readonly Action<PlayerStats> SetPermanentPlayerStats;

        private static readonly Func<Inventory> GetInventory;
        private static readonly Action<Inventory> SetInventory;

        private static readonly Func<GeneralSettings> GetGeneralSettings;
        private static readonly Action<GeneralSettings> SetGeneralSettings;

        private static readonly Action DelegateProgramStartInitialize;
        private static readonly Action DelegateSaveCombinedSaveFile;

        static PatchSaveLube()
        {
            var typeSaveLube = AccessTools.TypeByName("JumpKing.SaveThread.SaveLube");

            // Only the combined save uses a backing field.
            CombinedSaveRef =
                AccessTools.StaticFieldRefAccess<CombinedSaveFile>(AccessTools.Field(typeSaveLube, "_COMBINED_SAVE"));

            GetEventFlags = ReflectionUtil.CreateStaticPropertyGetter<EventFlagsSave>(typeSaveLube, "eventFlags");
            SetEventFlags = ReflectionUtil.CreateStaticPropertySetter<EventFlagsSave>(typeSaveLube, "eventFlags");

            GetGeneralSettings =
                ReflectionUtil.CreateStaticPropertyGetter<GeneralSettings>(typeSaveLube, "generalSettings");
            SetGeneralSettings =
                ReflectionUtil.CreateStaticPropertySetter<GeneralSettings>(typeSaveLube, "generalSettings");

            GetInventory = ReflectionUtil.CreateStaticPropertyGetter<Inventory>(typeSaveLube, "inventory");
            SetInventory = ReflectionUtil.CreateStaticPropertySetter<Inventory>(typeSaveLube, "inventory");

            GetPlayerStatsAttemptSnapshot =
                ReflectionUtil.CreateStaticPropertyGetter<PlayerStats>(typeSaveLube, "PlayerStatsAttemptSnapshot");
            SetPlayerStatsAttemptSnapshot =
                ReflectionUtil.CreateStaticPropertySetter<PlayerStats>(typeSaveLube, "PlayerStatsAttemptSnapshot");

            GetPermanentPlayerStats =
                ReflectionUtil.CreateStaticPropertyGetter<PlayerStats>(typeSaveLube, "PermanentPlayerStats");
            SetPermanentPlayerStats =
                ReflectionUtil.CreateStaticPropertySetter<PlayerStats>(typeSaveLube, "PermanentPlayerStats");

            DelegateProgramStartInitialize =
                (Action)typeSaveLube.GetMethod("ProgramStartInitialize").CreateDelegate(typeof(Action));

            DelegateSaveCombinedSaveFile =
                (Action)typeSaveLube.GetMethod("SaveCombinedSaveFile").CreateDelegate(typeof(Action));
        }

        public static CombinedSaveFile CombinedSaveFile
        {
            get => CombinedSaveRef();
            set => CombinedSaveRef() = value;
        }

        public static EventFlagsSave EventFlags
        {
            get => GetEventFlags();
            set => SetEventFlags(value);
        }

        public static GeneralSettings GeneralSettings
        {
            get => GetGeneralSettings();
            set => SetGeneralSettings(value);
        }

        public static Inventory Inventory
        {
            get => GetInventory();
            set => SetInventory(value);
        }

        public static PlayerStats PlayerStatsAttemptSnapshot
        {
            get => GetPlayerStatsAttemptSnapshot();
            set => SetPlayerStatsAttemptSnapshot(value);
        }

        public static PlayerStats PermanentPlayerStats
        {
            get => GetPermanentPlayerStats();
            set => SetPermanentPlayerStats(value);
        }

        public static void ProgramStartInitialize() =>
            DelegateProgramStartInitialize();

        public static void SaveCombinedSaveFile() =>
            DelegateSaveCombinedSaveFile();
    }
}
