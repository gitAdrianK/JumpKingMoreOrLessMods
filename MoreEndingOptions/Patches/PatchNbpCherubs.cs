// ReSharper disable InconsistentNaming

namespace MoreEndingOptions.Patches
{
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util.DrawBT;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPCherubs", "MakeBT")]
    public class PatchNbpCherubs
    {
        [UsedImplicitly]
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            if (ModEntry.ShortNewBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
            }
        }
    }
}
