namespace LessLocationText.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using HarmonyLib;

    [HarmonyPatch("JumpKing.MiscSystems.LocationText.LocationComp")]
    public class PatchLocationComp
    {
        [HarmonyPatch("PollCurrent")]
        [HarmonyPostfix]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
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
        public static void PatchPollNewScreen(ref bool __result)
        {
            if (ModEntry.Preferences.ShouldHideDiscover)
            {
                __result = false;
            }
        }
    }
}
