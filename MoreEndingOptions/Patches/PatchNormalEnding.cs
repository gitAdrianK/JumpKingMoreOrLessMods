namespace MoreEndingOptions.Patches
{
    using BehaviorTree;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding.NormalEnding;

    [HarmonyPatch(typeof(NormalEnding), "MakeBT")]
    public static class PatchNormalEnding
    {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref BTmanager __result)
        {
            if (ModEntry.ShortMainBabe)
            {
                __result = new BTmanager(
                    new BTsequencor(new BTPlayMusic(Game1.instance.contentManager.audio.music.Ending)));
            }
        }
    }
}
