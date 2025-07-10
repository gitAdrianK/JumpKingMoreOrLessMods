namespace MoreTextOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpriteBatch), nameof(SpriteBatch.DrawString), typeof(SpriteFont), typeof(string),
        typeof(Vector2), typeof(Color))]
    public static class PatchSpriteBatch
    {
        [UsedImplicitly]
        public static bool Prefix(SpriteFont spriteFont, ref string text, Vector2 position, ref Color color)
        {
            if (!ModEntry.Regex.IsMatch(text))
            {
                return true;
            }

            var pairs = Sanitize(ModEntry.Regex.Split(text));

            var colors = new List<string>();
            var texts = new List<string>();
            var i = 0;
            foreach (var element in pairs)
            {
                if (i % 2 == 0)
                {
                    colors.Add(element);
                }
                else
                {
                    texts.Add(element);
                }

                i++;
            }

            if (colors.Count != texts.Count)
            {
                return true;
            }

            text = texts.First();

            var advancedPosition = new Vector2(position.X + spriteFont.MeasureString(text).X, position.Y);
            for (var j = 1; j < texts.Count; j++)
            {
                var remainingColor = ColorFromHex(colors[j]);
                remainingColor = AdjustRgbaFromRef(remainingColor, color);
                Game1.spriteBatch.DrawString(spriteFont, texts[j], advancedPosition, remainingColor);
                advancedPosition.X += spriteFont.MeasureString(texts[j]).X;
            }

            var newColor = ColorFromHex(colors.First());
            newColor = AdjustRgbaFromRef(newColor, color);
            color = newColor;

            return true;
        }

        private static LinkedList<string> Sanitize(string[] substrings)
        {
            LinkedList<string> pairs;
            if (substrings[0] == string.Empty)
            {
                pairs = new LinkedList<string>(substrings.Skip(1));
            }
            else
            {
                pairs = new LinkedList<string>(substrings);
                _ = pairs.AddFirst("#FFFFFF");
            }

            return pairs;
        }

        private static Color ColorFromHex(string hex)
        {
            var r = Convert.ToInt32(hex.Substring(1, 2), 16);
            var g = Convert.ToInt32(hex.Substring(3, 2), 16);
            var b = Convert.ToInt32(hex.Substring(5, 2), 16);
            return new Color(r, g, b);
        }

        private static Color AdjustRgbaFromRef(Color color, Color reference)
        {
            var adjustedR = reference.R / 255.0f * color.R;
            var adjustedG = reference.G / 255.0f * color.G;
            var adjustedB = reference.B / 255.0f * color.B;
            return new Color(
                (int)adjustedR,
                (int)adjustedG,
                (int)adjustedB,
                reference.A);
        }
    }
}
