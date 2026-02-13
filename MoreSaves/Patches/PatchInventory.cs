namespace MoreSaves.Patches
{
    using HarmonyLib;

    [HarmonyPatch("JumpKing.SaveThread.SaveLube", "inventory", MethodType.Setter)]
    public static class PatchInventory
    {
        public static void Postfix()
        {
            var saveManager = ModEntry.SaveManager;
            saveManager.SaveInventory();
        }
    }
}
