namespace MoreSaves.Patches
{
    using HarmonyLib;

    [HarmonyPatch("JumpKing.SaveThread.SaveLube", "DeleteSaves")]
    public class PatchDeleteSaves
    {
        public static void Postfix() => ModEntry.SaveManager.DeleteAutoSave();
    }
}
