// ReSharper disable InconsistentNaming

namespace MoreEndingOptions.Patches
{
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util.DrawBT;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.OwlEnding.OwlBirdEntity", "MakeBT")]
    public static class PatchOwlBirdEntity
    {
        [UsedImplicitly]
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            if (ModEntry.ShortGhostBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
            }
        }
    }
}
