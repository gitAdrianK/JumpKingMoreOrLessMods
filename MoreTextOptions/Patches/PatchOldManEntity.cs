// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using EntityComponent;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.Props.RattmanText;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch("JumpKing.MiscEntities.OldManEntity", "GetOldManFont")]
    public static class PatchOldManEntity
    {
        private static bool HasCustomFonts { get; set; }

        private static SpriteFont[] Fonts { get; set; }

        [UsedImplicitly]
        public static bool Prefix(ref OldManFont p_font, ref SpriteFont __result)
        {
            var value = (int)p_font;
            if (!HasCustomFonts || value == 0 || value == 1)
            {
                return true;
            }

            var customFont = Fonts?[value];
            if (customFont is null)
            {
                p_font = OldManFont.Default;
                return true;
            }

            __result = customFont;
            return false;
        }

        /// <summary>
        ///     Loads all fonts inside the font folder as well assigning the corresponding id of the font
        ///     to entities that have been specified inside the OldManSettings.xml and RattmanSettings.xml.
        /// </summary>
        /// <param name="contentManager">JK Content Manager.</param>
        public static void LoadAndAssignFonts(JKContentManager contentManager)
        {
            HasCustomFonts = false;
            Fonts = null;

            var root = contentManager.root;
            var fontPath = Path.Combine(root, "font");

            if (!Directory.Exists(fontPath))
            {
                return;
            }

            var intToFontDictionary = new Dictionary<int, SpriteFont>();
            var nameToIntDictionary = new Dictionary<string, int>();

            var index = 2;
            foreach (var fontFile in Directory.GetFiles(fontPath, "*.xnb")
                         .Where(file => !(file.EndsWith("Default.xnb") || file.EndsWith("Gargoyle.xnb"))))
            {
                var name = Path.GetFileNameWithoutExtension(fontFile);
                nameToIntDictionary[name] = index;
                var pathWithoutExtension = Path.Combine(fontPath, name);
                intToFontDictionary[index] = contentManager.Load<SpriteFont>(pathWithoutExtension);
                index++;
            }

            if (index == 2)
            {
                // The index is two if no other files have been found.
                // That way we don't have to sift through all entities and look for files and what have you.
                return;
            }

            HasCustomFonts = true;
            Fonts = intToFontDictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();

            var oldManResult = LoadOldManSettings(fontPath, out var oldManSettings);
            var rattmanResult = LoadRattmanSettings(fontPath, out var rattmanSettings);
            // For additional safety, if someone makes a font folder
            // puts fonts inside but does not define any custom font mappings.
            if (!oldManResult && !rattmanResult)
            {
                return;
            }

            var oldManType = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");
            var oldManSettingsRef =
                AccessTools.FieldRefAccess<object, OldManSettings>(AccessTools.Field(oldManType, "m_settings"));
            var oldManNameRef = AccessTools.FieldRefAccess<object, string>(AccessTools.Field(oldManType, "m_name"));

            var rattmanType = AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity");
            var rattmanSettingsRef =
                AccessTools.FieldRefAccess<object, RattmanSettings>(AccessTools.Field(rattmanType, "m_settings"));

            foreach (var entity in EntityManager.instance.Entities)
            {
                if (entity.GetType() == oldManType)
                {
                    if (!oldManSettings.TryGetValue(oldManNameRef(entity), out var fontName))
                    {
                        continue;
                    }

                    if (!nameToIntDictionary.TryGetValue(fontName, out var fontId))
                    {
                        continue;
                    }

                    var settings = oldManSettingsRef(entity);
                    settings.font = (OldManFont)fontId;
                    oldManSettingsRef(entity) = settings;
                }

                // ReSharper disable once InvertIf
                if (entity.GetType() == rattmanType)
                {
                    var settings = rattmanSettingsRef(entity);
                    if (!rattmanSettings.TryGetValue(settings.screen, out var fontName))
                    {
                        continue;
                    }

                    if (!nameToIntDictionary.TryGetValue(fontName, out var fontId))
                    {
                        continue;
                    }

                    settings.font = (OldManFont)fontId;
                    rattmanSettingsRef(entity) = settings;
                }
            }
        }

        private static bool LoadOldManSettings(string fontPath, out Dictionary<string, string> settings)
        {
            settings = new Dictionary<string, string>();

            var xmlFile = Path.Combine(fontPath, "OldManSettings.xml");
            if (!File.Exists(xmlFile))
            {
                return false;
            }

            var root = XDocument.Load(xmlFile).Root;
            if (root == null)
            {
                return false;
            }

            var elements = root.Elements("OldManSetting");
            foreach (var element in elements)
            {
                var name = element.Element("Name");
                var font = element.Element("Font");

                if (name is null || font is null)
                {
                    continue;
                }

                settings[name.Value] = font.Value;
            }

            return true;
        }

        private static bool LoadRattmanSettings(string fontPath, out Dictionary<int, string> settings)
        {
            settings = new Dictionary<int, string>();

            var xmlFile = Path.Combine(fontPath, "RattmanSettings.xml");
            if (!File.Exists(xmlFile))
            {
                return false;
            }

            var root = XDocument.Load(xmlFile).Root;
            if (root == null)
            {
                return false;
            }

            var elements = root.Elements("RattmanSetting");
            foreach (var element in elements)
            {
                var screen = element.Element("Screen");
                var font = element.Element("Font");

                if (screen is null || font is null)
                {
                    continue;
                }

                settings[int.Parse(screen.Value)] = font.Value;
            }

            return true;
        }
    }
}
