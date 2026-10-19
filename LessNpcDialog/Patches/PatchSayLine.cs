namespace LessNpcDialog.Patches
{
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(nameof(SayLine), "MyRun")]
    public static class PatchSayLine
    {
        // ReSharper disable InconsistentNaming
        public static bool Prefix(ref BTresult __result)
        {
            if (!ModEntry.Preferences.IsEnabled)
            {
                return true;
            }

            __result = BTresult.Success;
            return false;
        }
    }
}
