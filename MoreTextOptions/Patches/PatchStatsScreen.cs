// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager;
    using JumpKing.GameManager.MultiEnding;

    [HarmonyPatch(typeof(StatsScreen), "GetEnding")]
    public static class PatchStatsScreen
    {
        private static string MainBabeEnding { get; set; }
        private static string NewBabeEnding { get; set; }
        private static string GhostBabeEnding { get; set; }

        [UsedImplicitly]
        public static void Postfix(ref string __result)
        {
            if (Game1.instance.contentManager.root == "Content")
            {
                return;
            }

            var ending = GameEnding.GetEnding();
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (ending == EndingType.Normal && MainBabeEnding != string.Empty)
            {
                __result = MainBabeEnding;
            }
            else if (ending == EndingType.NewBabePlus && NewBabeEnding != string.Empty)
            {
                __result = NewBabeEnding;
            }
            else if (ending == EndingType.Ghost && GhostBabeEnding != string.Empty)
            {
                __result = GhostBabeEnding;
            }
        }

        public static void LoadEndingNames()
        {
            MainBabeEnding = string.Empty;
            NewBabeEnding = string.Empty;
            GhostBabeEnding = string.Empty;
            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            var mainBabeRegex = new Regex("^MainBabeEndingName=(.*)$");
            var newBabeRegex = new Regex("^NewBabeEndingName=(.*)$");
            var ghostBabeRegex = new Regex("^GhostBabeEndingName=(.*)$");
            foreach (var tag in tags)
            {
                var mainMatch = mainBabeRegex.Match(tag);
                if (mainMatch.Success)
                {
                    MainBabeEnding = mainMatch.Groups[1].Value;
                }

                var newMatch = newBabeRegex.Match(tag);
                if (newMatch.Success)
                {
                    NewBabeEnding = newMatch.Groups[1].Value;
                }

                var ghostMatch = ghostBabeRegex.Match(tag);
                if (ghostMatch.Success)
                {
                    GhostBabeEnding = ghostMatch.Groups[1].Value;
                }
            }
        }
    }
}
