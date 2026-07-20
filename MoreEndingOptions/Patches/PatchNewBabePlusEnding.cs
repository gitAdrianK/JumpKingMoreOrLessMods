namespace MoreEndingOptions.Patches
{
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;

    [HarmonyPatch(typeof(NewBabePlusEnding), "MakeBT")]
    public static class PatchNewBabePlusEnding
    {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref BTmanager __result)
        {
            if (ModEntry.ShortNewBabe)
            {
                __result = new BTmanager(
                    new BTsequencor(new BTPlayMusic(Game1.instance.contentManager.audio.music.Ending2)));
            }
        }
    }
}
