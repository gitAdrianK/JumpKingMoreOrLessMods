namespace MoreEndingOptions.Patches
{
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util.DrawBT;
    using Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPKingEntity", "MakeBT")]
    public static class PatchNbpKingEntity
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            if (ModEntry.ShortNewBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
                return;
            }

            if (ModEntry.MuteNewBabe)
            {
                /*
                 Sounds, in order played, are:
                 1 - player.EndingParasol
                 2 - babe.Jump
                 3 - babe.Surprised
                 4 - player.Jump
                 5 - babe.Scream
                */
                BtWalker.FilterRecursively(__result, BtWalker.PredicateMute);
            }
        }
    }
}
