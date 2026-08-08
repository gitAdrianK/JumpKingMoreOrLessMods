namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.Merchant;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.MiscSystems.LocationText;
    using JumpKing.Props.RattmanText;
    using JumpKing.Workshop;
    using Steamworks;

    [HarmonyPatch]
    public static class PatchXmlSerializerHelper
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var deserialize = AccessTools.Method(
                typeof(XmlSerializerHelper),
                nameof(XmlSerializerHelper.Deserialize));
            yield return deserialize.MakeGenericMethod(typeof(Level.LevelSettings));
            yield return deserialize.MakeGenericMethod(typeof(LocationSettings));
            yield return deserialize.MakeGenericMethod(typeof(RattmanSettings));
            yield return deserialize.MakeGenericMethod(typeof(OldManSettings));
            yield return deserialize.MakeGenericMethod(typeof(MerchantSettings));
        }

        [HarmonyPrefix]
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
