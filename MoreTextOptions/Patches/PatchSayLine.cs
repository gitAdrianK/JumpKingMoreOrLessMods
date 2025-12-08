// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(typeof(SayLine), "MyRun")]
    public static class PatchSayLine
    {
        private static readonly AccessTools.FieldRef<SayLine, string> CurrentLineRef =
            AccessTools.FieldRefAccess<SayLine, string>(
                AccessTools.Field("JumpKing.MiscEntities.OldMan:m_current_line"));

        // Basically ModEntry regex, but requires it to be at the start and no first '{'.
        private static readonly Regex PrefixRegex =
            new Regex("^color=\"(#(?:[0-9a-fA-F]{2}){3})\"}", RegexOptions.IgnoreCase);

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void Postfix(SayLine __instance, BTresult __result)
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
            if (!PrefixRegex.IsMatch(remainder))
            {
                return;
            }

            // The regex is 16 characters long,
            // but we are also adding the next character (17).
            if (remainder.Length >= 17)
            {
                CurrentLineRef(__instance) = currentLine + remainder.Substring(0, 17);
            }
        }
    }
}
