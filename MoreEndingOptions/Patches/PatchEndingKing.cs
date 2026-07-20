namespace MoreEndingOptions.Patches
{
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util.DrawBT;
    using Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NormalEnding.EndingKing", "MakeBT")]
    public static class PatchEndingKing
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            if (ModEntry.ShortMainBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
                return;
            }

            if (ModEntry.MuteMainBabe)
            {
                /*
                 Sounds, in order played, are:
                 1 - player.Jump
                 2 - audio.Plink
                 3 - player.Land
                 4 - player.EndingParasol
                 5 - babe.Surprised
                 6 - babe.Surprised
                 7 - player.Jump
                 8 - babe.Scream

                 1 - babe.Mou
                */
                BtWalker.FilterRecursively(__result, BtWalker.PredicateMute);
            }
        }
    }
}
