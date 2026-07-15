// ReSharper disable InconsistentNaming

namespace LessLocationText.Patches
{
    using HarmonyLib;
    using JetBrains.Annotations;

    public static class PatchLocationComp
    {
        [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp", "PollCurrent")]
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void PatchPollCurrent(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideEnter)
            {
                __result = false;
            }
        }

        [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp", "PollNewScreen")]
        [HarmonyPostfix]
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
