namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.MiscEntities.Merchant;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.MiscSystems.LocationText;
    using JumpKing.Mods;
    using JumpKing.Props.RattmanText;
    using JumpKing.Workshop;
    using Microsoft.Xna.Framework.Content;
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

                var contentManager = Game1.instance.contentManager;
                contentManager.oldMan.Reload(contentManager);
                contentManager.props.Reload(contentManager);
                contentManager.miscSettings.Load(Traverse.Create(contentManager).Field("contentManager")
                    .GetValue<ContentManager>());
            }
            catch
            {
                // Traverse/.Invoke is okay because it happens only once.
                var instance = AccessTools.StaticFieldRefAccess<ModLoader>("JumpKing.Mods.ModLoader:instance");
                var method = AccessTools.Method("JumpKing.Mods.ModLoader:WriteLoadLogs",
                    new[] { typeof(List<string>), typeof(bool) });
                method.Invoke(instance,
                    new object[]
                    {
                        new List<string>
                        {
                            "[ERROR - MoreTextOptions] Generic patching failed, translations feature unavailable.",
                            "[FIX] You will have to change the mod load order to load a mod that includes harmony2.2.2 first.",
                            "[HINT] This mod includes harmony 2.2.2, all of my mods do :D",
                        },
                        false,
                    });
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
