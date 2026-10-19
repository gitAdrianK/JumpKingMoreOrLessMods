namespace MoreTextOptions.Patches
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager;
    using Steamworks;

    [HarmonyPatch(typeof(IntroState), "InitializeEntities")]
    public static class PatchIntroState
    {
        public static void Prefix()
        {
            ModEntry.Translations = null;

            var path = Path.Combine(Game1.instance.contentManager.root, "language.xml");
            if (!File.Exists(path))
            {
                return;
            }

            var currentLanguage = SteamApps.GetCurrentGameLanguage();
            var doc = XDocument.Load(path);

            ModEntry.Translations = doc.Root
                .Elements()
                .ToDictionary(
                    entry => entry.Name.LocalName,
                    entry =>
                        entry.Element(currentLanguage)?.Value
                        ?? entry.Element("english")?.Value
                        ?? entry.Value
                );
        }
    }
}
