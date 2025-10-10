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

        private static SpriteFont[] Fonts { get; set; } =
        {
            Game1.instance.contentManager.font.StyleFont, Game1.instance.contentManager.font.GargoyleFont
        };

        [UsedImplicitly]
        public static bool Prefix(OldManFont p_font, ref SpriteFont __result)
        {
            if (!HasCustomFonts)
            {
                return true;
            }

            __result = Fonts?[(int)p_font] ?? Game1.instance.contentManager.font.StyleFont;
            return false;
        }

        /// <summary>
        /// Loads all fonts inside the font folder as well assigning the corresponding id of the font
        /// to entities that have been specified inside the OldManSettings.xml and RattmanSettings.xml.
        /// </summary>
        /// <param name="contentManager">JK Content Manager.</param>
        public static void LoadAndAssignFonts(JKContentManager contentManager)
        {
            HasCustomFonts = false;

            var root = contentManager.root;
            var fontPath = Path.Combine(root, "font");

            if (!Directory.Exists(fontPath))
            {
                return;
            }

            HasCustomFonts = true;
            var intToFontDictionary = new Dictionary<int, SpriteFont>();
            var nameToIntDictionary = new Dictionary<string, int>();

            intToFontDictionary[0] = File.Exists(Path.Combine(fontPath, "Default.xnb"))
                ? contentManager.Load<SpriteFont>(Path.Combine(fontPath, "Default"))
                : contentManager.font.StyleFont;

            intToFontDictionary[1] = File.Exists(Path.Combine(fontPath, "Gargoyle.xnb"))
                ? contentManager.Load<SpriteFont>(Path.Combine(fontPath, "Gargoyle"))
                : contentManager.font.GargoyleFont;

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

            Fonts = intToFontDictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();

            if (index == 2)
            {
                // The index is two if no other files have been found.
                // That way we don't have to sift through all entities and look for files and what have you.
                return;
            }

            var oldManSettings = LoadOldManSettings(fontPath);
            var rattmanSettings = LoadRattmanSettings(fontPath);

            var oldManType = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");
            var rattmanType = AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity");

            foreach (var entity in EntityManager.instance.Entities)
            {
                if (entity.GetType() == oldManType)
                {
                    var traverse = Traverse.Create(entity);
                    if (!oldManSettings.TryGetValue(traverse.Field("m_name").GetValue<string>(), out var fontName))
                    {
                        continue;
                    }

                    if (!nameToIntDictionary.TryGetValue(fontName, out var fontId))
                    {
                        continue;
                    }

                    var settings = traverse.Field("m_settings").GetValue<OldManSettings>();
                    settings.font = (OldManFont)fontId;
                    traverse.Field("m_settings").SetValue(settings);
                }

                // ReSharper disable once InvertIf
                if (entity.GetType() == rattmanType)
                {
                    var traverse = Traverse.Create(entity);
                    if (!rattmanSettings.TryGetValue(traverse.Field("m_settings").Field("screen").GetValue<int>(),
                            out var fontName))
                    {
                        continue;
                    }

                    if (!nameToIntDictionary.TryGetValue(fontName, out var fontId))
                    {
                        continue;
                    }

                    var settings = traverse.Field("m_settings").GetValue<RattmanSettings>();
                    settings.font = (OldManFont)fontId;
                    traverse.Field("m_settings").SetValue(settings);
                }
            }
        }

        private static Dictionary<string, string> LoadOldManSettings(string fontPath)
        {
            var settings = new Dictionary<string, string>();

            var xmlFile = Path.Combine(fontPath, "OldManSettings.xml");
            if (!File.Exists(xmlFile))
            {
                return settings;
            }

            var root = XDocument.Load(xmlFile).Root;
            if (root == null)
            {
                return settings;
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

            return settings;
        }

        private static Dictionary<int, string> LoadRattmanSettings(string fontPath)
        {
            var settings = new Dictionary<int, string>();

            var xmlFile = Path.Combine(fontPath, "RattmanSettings.xml");
            if (!File.Exists(xmlFile))
            {
                return settings;
            }

            var root = XDocument.Load(xmlFile).Root;
            if (root == null)
            {
                return settings;
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

            return settings;
        }
    }
}
