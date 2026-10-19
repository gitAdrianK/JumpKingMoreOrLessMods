// ReSharper disable InconsistentNaming

namespace LessLocationText.Patches
{
    using HarmonyLib;

    public static class PatchLocationComp
    {
        [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp", "PollCurrent")]
        [HarmonyPostfix]
        public static void PatchPollCurrent(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideEnter)
            {
                __result = false;
            }
        }

        [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp", "PollNewScreen")]
        [HarmonyPostfix]
        public static void PatchPollNewScreen(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideDiscover)
            {
                __result = false;
            }
        }
    }
}
