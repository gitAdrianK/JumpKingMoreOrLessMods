// ReSharper disable InconsistentNaming

namespace LessLocationText.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using HarmonyLib;
    using JetBrains.Annotations;

    [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp")]
    public static class PatchLocationComp
    {
        [HarmonyPatch("PollCurrent")]
        [HarmonyPostfix]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void PatchPollCurrent(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideEnter)
            {
                __result = false;
            }
        }

        [HarmonyPatch("PollNewScreen")]
        [HarmonyPostfix]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void PatchPollNewScreen(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideDiscover)
            {
                __result = false;
            }
        }
    }
}
