// ReSharper disable InconsistentNaming

namespace LessNpcDialog.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch(nameof(SayLine), "MyRun")]
    public static class PatchSayLine
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
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
