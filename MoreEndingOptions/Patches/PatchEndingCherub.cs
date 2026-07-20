// ReSharper disable InconsistentNaming

namespace MoreEndingOptions.Patches
{
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util.DrawBT;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NormalEnding.EndingCherub", "MakeBT")]
    public static class PatchEndingCherub
    {
        [UsedImplicitly]
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            if (ModEntry.ShortMainBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
            }
        }
    }
}
