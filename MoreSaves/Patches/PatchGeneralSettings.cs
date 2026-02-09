namespace MoreSaves.Patches
{
    using HarmonyLib;

    [HarmonyPatch("JumpKing.SaveThread.SaveLube", "generalSettings", MethodType.Setter)]
    public static class PatchGeneralSettings
    {
        public static void Postfix() =>
            ModEntry.SaveManager.SaveGeneralSettings(ModEntry.SaveManager.AutoDirectory, ModEntry.SaveManager.SaveName);
    }
}
