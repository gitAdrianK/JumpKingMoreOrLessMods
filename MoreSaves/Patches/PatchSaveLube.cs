// ReSharper disable InconsistentNaming

namespace MoreSaves.Patches
{
    using System.IO;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.SaveThread;

    /// <summary>
    ///     Patches the SaveLube class.
    ///     Function SaveCombinedSaveFile to also save at our mod location.
    ///     Function DeleteSaves to also delete the saves inside the auto folder.
    /// </summary>
    public class PatchSaveLube
    {
        private static readonly char Sep;

        private static readonly Traverse CombinedSavefile;
        private static readonly Traverse GeneralSettings;

        private static readonly MethodInfo MethodSaveCombinedSaveFile;
        private static readonly HarmonyMethod MethodSaveCombinedPatch;

        private static readonly MethodInfo MethodDeleteSave;
        private static readonly HarmonyMethod MethodDeletePatch;

        static PatchSaveLube()
        {
            Sep = Path.DirectorySeparatorChar;

            var saveLube = AccessTools.TypeByName("JumpKing.SaveThread.SaveLube");

            CombinedSavefile = Traverse.Create(saveLube).Property("CombinedSave");
            GeneralSettings = Traverse.Create(saveLube).Property("generalSettings");

            var genericSave = saveLube.GetMethod("Save");
            MethodSaveCombinedSaveFile = genericSave?.MakeGenericMethod(typeof(CombinedSaveFile));
            MethodSaveCombinedPatch = new HarmonyMethod(typeof(PatchSaveLube).GetMethod(nameof(SaveCombinedSaveFile)));

            MethodDeleteSave = saveLube.GetMethod("DeleteSaves");
            MethodDeletePatch = new HarmonyMethod(typeof(PatchSaveLube).GetMethod(nameof(DeleteSaves)));
        }

        public PatchSaveLube(Harmony harmony)
        {
            _ = harmony.Patch(
                MethodSaveCombinedSaveFile,
                postfix: MethodSaveCombinedPatch
            );

            _ = harmony.Patch(
                MethodDeleteSave,
                postfix: MethodDeletePatch
            );
        }

        public static void SaveCombinedSaveFile(CombinedSaveFile p_object)
        {
            if (ModEntry.SaveName == string.Empty)
            {
                return;
            }

            PatchEncryption.SaveCombinedSaveFile(p_object, ModStrings.Auto, ModEntry.SaveName, ModStrings.Saves);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPermaStats(), ModStrings.Permanent,
                ModStrings.Auto, ModEntry.SaveName, ModStrings.SavesPerma);
        }

        /// <summary>
        ///     Deletes the savefiles in the dll directory when the give up option in selected in game.
        /// </summary>
        public static void DeleteSaves()
        {
            if (ModEntry.SaveName == string.Empty)
            {
                return;
            }

            var directory = $"{ModEntry.DllDirectory}{Sep}{ModStrings.Auto}{Sep}{ModEntry.SaveName}{Sep}";
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            ModEntry.SaveName = string.Empty;
        }

        public static CombinedSaveFile GetCombinedSaveFile()
            => CombinedSavefile.GetValue<CombinedSaveFile>();

        public static GeneralSettings GetGeneralSettings()
            => GeneralSettings.GetValue<GeneralSettings>();
    }
}
