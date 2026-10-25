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
                @"^Hide(?<raven>.+?)(?<condition>Until|At)Position=\(\s*(?<position>\d+(?:\s*,\s*\d+)*)\s*\)$");

            foreach (var tag in tags)
            {
                var matchRavenHide = regexRavenHide.Match(tag);
                if (!matchRavenHide.Success)
                {
                    continue;
                }

                if (PatchRavenEntity.HideScreens == null)
                {
                    PatchRavenEntity.HideScreens = new Dictionary<string, HashSet<int>>();
                }

                var name = PatchRavenEntity.NameToInternal(matchRavenHide.Groups["raven"].Value);
                var condition = matchRavenHide.Groups["condition"].Value;
                var positions = matchRavenHide.Groups["position"].Value;
                var screens = new HashSet<int>();

                switch (condition)
                {
                    case "Until":
                        for (var i = 0; i < int.Parse(positions); i++)
                        {
                            screens.Add(i - 1);
                        }

                        break;

                    case "At":
                        foreach (var pos in positions.Split(','))
                        {
                            if (int.TryParse(pos.Trim(), out var position))
                            {
                                screens.Add(position - 1);
                            }
                        }

                        break;
                }

                PatchRavenEntity.HideScreens[name] = screens;
            }
        }
    }
}
