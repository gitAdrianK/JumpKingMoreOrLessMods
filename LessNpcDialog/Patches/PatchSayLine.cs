namespace LessNpcDialog.Patches
{
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(nameof(SayLine), "MyRun")]
    public static class PatchSayLine
    {
        [UsedImplicitly]
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
