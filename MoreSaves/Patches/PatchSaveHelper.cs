namespace MoreSaves.Patches
{
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems.Inventory;
    using JumpKing.MiscSystems.Achievements;
    using JumpKing.SaveThread;

    public class PatchSaveHelper
    {
        private static readonly MethodInfo MethodSaveGeneralSettings;
        private static readonly HarmonyMethod MethodSaveGeneralSettingsPatch;

        private static readonly MethodInfo MethodSaveStats;
        private static readonly HarmonyMethod MethodSaveStatsPatch;

        private static readonly MethodInfo MethodSaveInventory;
        private static readonly HarmonyMethod MethodSaveInventoryPatch;

        static PatchSaveHelper()
        {
            var saveHelper = AccessTools.TypeByName("JumpKing.SaveThread.SaveHelper");

            var save = saveHelper.GetMethod("Save");
            var saveEncrypted = saveHelper.GetMethod("SaveEncrypted");

            MethodSaveGeneralSettings = save.MakeGenericMethod(typeof(GeneralSettings));
            MethodSaveStats = saveEncrypted.MakeGenericMethod(typeof(PlayerStats));
            MethodSaveInventory = saveEncrypted.MakeGenericMethod(typeof(Inventory));

            MethodSaveGeneralSettingsPatch = new HarmonyMethod(AccessTools.Method(typeof(PatchSaveHelper), nameof(SaveGeneralSettings)));
            MethodSaveStatsPatch = new HarmonyMethod(AccessTools.Method(typeof(PatchSaveHelper), nameof(SaveStats)));
            MethodSaveInventoryPatch = new HarmonyMethod(AccessTools.Method(typeof(PatchSaveHelper), nameof(SaveInventory)));
        }

        public PatchSaveHelper(Harmony harmony)
        {
            _ = harmony.Patch(
                MethodSaveGeneralSettings,
                postfix: MethodSaveGeneralSettingsPatch);

            _ = harmony.Patch(
                MethodSaveStats,
                postfix: MethodSaveStatsPatch);

            _ = harmony.Patch(
                MethodSaveInventory,
                postfix: MethodSaveInventoryPatch);
        }


        public static void SaveGeneralSettings()
        {
            if (ModEntry.SaveName == string.Empty)
            {
                return;
            }
            PatchXmlWrapper.Serialize(PatchSaveLube.GetGeneralSettings(), ModStrings.AUTO, ModEntry.SaveName, ModStrings.SAVES_PERMA);
        }

        public static void SaveStats(string p_file, PlayerStats p_object)
        {
            if (ModEntry.SaveName == string.Empty)
            {
                return;
            }
            PatchEncryption.SavePlayerStats(p_object, p_file, ModStrings.AUTO, ModEntry.SaveName, ModStrings.SAVES_PERMA);
        }

        public static void SaveInventory(Inventory p_object)
        {
            if (ModEntry.SaveName == string.Empty)
            {
                return;
            }
            PatchEncryption.SaveInventory(p_object, ModStrings.AUTO, ModEntry.SaveName, ModStrings.SAVES_PERMA);
        }
    }
}
