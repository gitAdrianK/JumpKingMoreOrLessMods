namespace MoreEntityOptions.Patches
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager;

    [HarmonyPatch(typeof(IntroState), "InitializeEntities")]
    public static class PatchIntroState
    {
        public static void Postfix()
        {
            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            var regexRavenHide = new Regex(
                "^Hide(?<raven>WhiteRaven|Raven|Tsuchinoko|Fly)UntilPosition(?<position>\\d+)$");

            foreach (var tag in tags)
            {
                var matchRavenHide = regexRavenHide.Match(tag);
                if (!matchRavenHide.Success)
                {
                    continue;
                }

                if (PatchRavenEntity.HideUntil == null)
                {
                    PatchRavenEntity.HideUntil = new Dictionary<string, int>();
                }

                var raven = PatchRavenEntity.NameToInternal(matchRavenHide.Groups["raven"].Value);
                var positionValue = int.Parse(matchRavenHide.Groups["position"].Value) - 1;
                PatchRavenEntity.HideUntil[raven] = positionValue;
            }
        }
    }
}
