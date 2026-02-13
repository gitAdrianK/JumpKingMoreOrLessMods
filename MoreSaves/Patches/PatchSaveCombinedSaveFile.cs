namespace MoreSaves.Patches
{
    using HarmonyLib;

    [HarmonyPatch("JumpKing.SaveThread.SaveLube", "SaveCombinedSaveFile")]
    public static class PatchSaveCombinedSaveFile
    {
        public static void Postfix()
        {
            var saveManager = ModEntry.SaveManager;
            saveManager.SaveCombined();
            saveManager.SavePermaPlayerStats();
        }
    }
}
