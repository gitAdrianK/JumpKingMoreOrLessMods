namespace MoreSaves.Patches
{
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.MiscSystems.Achievements;

    public class PatchAchievementManager
    {
        private static readonly Traverse PlayerStats;
        private static readonly Traverse PermaStats;

        static PatchAchievementManager()
        {
            var achievementManager = AccessTools.TypeByName("JumpKing.MiscSystems.Achievements.AchievementManager");

            var achievementManagerInstance = achievementManager.GetField("instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var achievementManagerTraverse = Traverse.Create(achievementManagerInstance);

            PlayerStats = achievementManagerTraverse.Field("m_snapshot");
            PermaStats = achievementManagerTraverse.Field("m_all_time_stats");
        }

        public static PlayerStats GetPlayerStats() => PlayerStats.GetValue<PlayerStats>();

        public static PlayerStats GetPermaStats() => PermaStats.GetValue<PlayerStats>();
    }
}
