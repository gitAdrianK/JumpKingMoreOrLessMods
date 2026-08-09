namespace MoreTextOptions.Patches
{
    using System.IO;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.Merchant;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.MiscSystems.LocationText;
    using JumpKing.Props.RattmanText;
    using JumpKing.Workshop;
    using Steamworks;

    public static class PatchXmlSerializerHelper
    {
        public static void ApplyPatches(Harmony harmony)
        {
            var deserialize = AccessTools.Method(
                typeof(XmlSerializerHelper),
                nameof(XmlSerializerHelper.Deserialize));
            var deserializeLevel = deserialize.MakeGenericMethod(typeof(Level.LevelSettings));
            var deserializeLocation = deserialize.MakeGenericMethod(typeof(LocationSettings));
            var deserializeRattman = deserialize.MakeGenericMethod(typeof(RattmanSettings));
            var deserializeOldMan = deserialize.MakeGenericMethod(typeof(OldManSettings));
            var deserializeMerchant = deserialize.MakeGenericMethod(typeof(MerchantSettings));

            var harmonyMethod = new HarmonyMethod(typeof(PatchXmlSerializerHelper), nameof(InsertLanguage));

            try
            {
                harmony.Patch(deserializeLevel, harmonyMethod);
                harmony.Patch(deserializeLocation, harmonyMethod);
                harmony.Patch(deserializeRattman, harmonyMethod);
                harmony.Patch(deserializeOldMan, harmonyMethod);
                harmony.Patch(deserializeMerchant, harmonyMethod);
            }
            catch
            {
                // ignored
            }
        }

        [UsedImplicitly]
        public static void InsertLanguage(ref string path)
        {
            var currentLanguage = SteamApps.GetCurrentGameLanguage();
            if (string.IsNullOrEmpty(currentLanguage))
            {
                return;
            }

            var newPath = Regex.Replace(path, "\\.xml$", $".{currentLanguage}");
            if (!File.Exists(newPath))
            {
                return;
            }

            path = newPath;
        }
    }
}
