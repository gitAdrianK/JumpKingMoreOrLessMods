namespace MoreSaves.Patches
{
    using HarmonyLib;

    [HarmonyPatch("JumpKing.SaveThread.SaveLube", "SaveCombinedSaveFile")]
    public static class PatchSaveCombinedSaveFile
    {
        public static void Postfix()
        {
            var saveManager = ModEntry.SaveManager;
            saveManager.SaveCombined(saveManager.AutoDirectory, saveManager.SaveName);
            saveManager.SavePermaPlayerStats(saveManager.AutoDirectory, saveManager.SaveName);
        }
    }
}
