namespace LessNpcDialog.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(nameof(SayLine), "MyRun")]
    public class PatchSayLine
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        public static bool Prefix(ref BTresult __result)
        {
            if (ModEntry.Preferences.IsEnabled)
            {
                __result = BTresult.Success;
                return false;
            }
            return true;
        }
    }
}
