namespace MoreEndingOptions.Patches
{
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding.OwlEnding;

    [HarmonyPatch(typeof(OwlEnding), "MakeBT")]
    public static class PatchOwlEnding
    {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref BTmanager __result)
        {
            if (ModEntry.ShortGhostBabe)
            {
                __result = new BTmanager(
                    new BTsequencor(new BTPlayMusic(Game1.instance.contentManager.audio.music.Ending3)));
            }
        }
    }
}
