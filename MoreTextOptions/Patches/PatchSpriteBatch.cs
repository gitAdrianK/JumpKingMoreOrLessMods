namespace MoreTextOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using HarmonyLib;
    using JumpKing;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpriteBatch), nameof(SpriteBatch.DrawString), typeof(SpriteFont), typeof(string),
        typeof(Vector2), typeof(Color))]
    public static class PatchSpriteBatch
    {
        // We can't clear this OnLevelEnd because credits might use custom fonts which run after the OnLevelEnd step.
        private static Dictionary<string, SpriteFont> SpriteFonts { get; set; }

        public static void Prefix(ref SpriteFont spriteFont, ref string text, Vector2 position, ref Color color)
        {
            var matches = ModEntry.Regex.Matches(text);
            if (matches.Count == 0)
            {
                return;
            }

            var index = 0;
            var segments = ModEntry.Regex.Split(text);
            if (!string.IsNullOrEmpty(segments[0]))
            {
                text = segments[0];
            }
            else
            {
                text = segments[1];
                index = 1;

                var tag = matches[0].Value;
                var fontMatch = ModEntry.RegexFont.Match(tag);
                var colorMatch = ModEntry.RegexColor.Match(tag);

                if (fontMatch.Success)
                {
                    spriteFont = GetFont(fontMatch.Groups["Font"].Value) ?? spriteFont;
                }

                if (colorMatch.Success)
                {
                    color = AdjustRgbaFromRef(ColorFromHex(colorMatch.Groups["R"].Value,
                            colorMatch.Groups["G"].Value,
                            colorMatch.Groups["B"].Value),
                        color);
                }
            }

            var advancedPosition = new Vector2(position.X + spriteFont.MeasureString(text).X, position.Y);
            var currentFont = spriteFont;
            var currentColor = color;
            for (var i = index; i < matches.Count; i++)
            {
                var tag = matches[i].Value;
                var fontMatch = ModEntry.RegexFont.Match(tag);
                var colorMatch = ModEntry.RegexColor.Match(tag);

                if (fontMatch.Success)
                {
                    currentFont = GetFont(fontMatch.Groups["Font"].Value) ?? currentFont;
                }

                if (colorMatch.Success)
                {
                    currentColor = AdjustRgbaFromRef(ColorFromHex(colorMatch.Groups["R"].Value,
                            colorMatch.Groups["G"].Value,
                            colorMatch.Groups["B"].Value),
                        color);
                }

                var segment = segments[i + 1];
                if (string.IsNullOrEmpty(segment))
                {
                    return;
                }

                Game1.spriteBatch.DrawString(currentFont, segment, advancedPosition, currentColor);
                advancedPosition.X += currentFont.MeasureString(segment).X;
            }
        }

        private static Color ColorFromHex(string r, string g, string b) =>
            new Color(Convert.ToByte(r, 16), Convert.ToByte(g, 16), Convert.ToByte(b, 16));

        private static Color AdjustRgbaFromRef(Color color, Color reference) =>
            new Color(
                color.R * reference.R / 255,
                color.G * reference.G / 255,
                color.B * reference.B / 255,
                color.A * reference.A / 255
            );

        public static void LoadCustomFonts(string root)
        {
            var fontFolder = Path.Combine(root, "font");
            if (!Directory.Exists(fontFolder))
            {
                return;
            }

            SpriteFonts = new Dictionary<string, SpriteFont>();
            var contentManager = Game1.instance.contentManager;
            foreach (var file in Directory.EnumerateFiles(fontFolder, "*.xnb"))
            {
                var fontName = Path.GetFileNameWithoutExtension(file);
                var fontPath = Path.ChangeExtension(file, null);
                var spriteFont = contentManager.Load<SpriteFont>(fontPath);
                SpriteFonts[fontName.ToLowerInvariant()] = spriteFont;
            }
        }

        private static SpriteFont GetFont(string fontName)
        {
            var font = Game1.instance.contentManager.font;
            fontName = fontName.ToLowerInvariant();
            switch (fontName)
            {
                case "menufont":
                    return font.MenuFont;
                case "menufontsmall":
                    return font.MenuFontSmall;
                case "stylefont":
                    return font.StyleFont;
                case "optimusunderline":
                    return font.OptimusUnderline;
                case "tangerine":
                    return font.Tangerine;
                case "locationfont":
                    return font.LocationFont;
                case "gargoylefont":
                    return font.GargoyleFont;
            }

            return SpriteFonts.TryGetValue(fontName, out var spriteFont)
                ? spriteFont
                : font.MenuFont;
        }
    }
}
