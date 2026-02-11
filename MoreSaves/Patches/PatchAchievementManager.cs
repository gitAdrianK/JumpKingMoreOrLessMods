namespace SwitchBlocks.Patches
{
    using HarmonyLib;
    using JumpKing.MiscSystems.Achievements;

    public static class PatchAchievementManager
    {
        /// <summary>The achievement manager instance.</summary>
        private static readonly object AchievementManager =
            AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:instance").GetValue(null);

        /// <summary>FieldRef of the "all-time stats" field.</summary>
        private static readonly AccessTools.FieldRef<object, PlayerStats> AllTimeStatsRef =
            AccessTools.FieldRefAccess<object, PlayerStats>(
                AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:m_all_time_stats"));

        /// <summary>FieldRef of the "snapshot" field.</summary>
        private static readonly AccessTools.FieldRef<object, PlayerStats> SnapshotRef =
            AccessTools.FieldRefAccess<object, PlayerStats>(
                AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:m_snapshot"));

        public static PlayerStats AllTimeStats
        {
            get => AllTimeStatsRef(AchievementManager);
            set => AllTimeStatsRef(AchievementManager) = value;
        }

        public static PlayerStats Snapshot
        {
            get => SnapshotRef(AchievementManager);
            set => SnapshotRef(AchievementManager) = value;
        }
    }
}
