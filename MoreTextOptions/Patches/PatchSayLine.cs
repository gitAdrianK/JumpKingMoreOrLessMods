namespace MoreTextOptions.Patches
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(typeof(SayLine), "MyRun")]
    public static class PatchSayLine
    {
        // Basically ModEntry regex, but requires it to be at the start and no first '{'.
        private static readonly Regex PrefixRegex = new Regex(
            @"^(?:color=""#(?:[0-9a-fA-F]{2}){3}""|font=""[^""]+"")(?:\s+(?:color=""#(?:[0-9a-fA-F]{2}){3}""|font=""[^""]+""))?\}",
            RegexOptions.IgnoreCase
        );

        // ReSharper disable InconsistentNaming
        public static void Postfix(SayLine __instance, BTresult __result, ref string ___m_current_line)
        {
            if (__result == BTresult.Success)
            {
                return;
            }

            var currentLine = __instance.GetLine();
            if (currentLine.LastOrDefault() != '{')
            {
                return;
            }

            var sampleLine = __instance.GetFullLine();
            var remainder = sampleLine.Substring(currentLine.Length);
            var match = PrefixRegex.Match(remainder);
            if (!match.Success)
            {
                return;
            }

            // The regex is x characters long,
            // but we are also adding the next character (x + 1).
            if (remainder.Length >= match.Length)
            {
                ___m_current_line = currentLine + remainder.Substring(0, match.Length + 1);
            }
        }
    }
}
