// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(TextHelper), nameof(TextHelper.DrawString))]
    public static class PatchTextHelper
    {
        [UsedImplicitly]
        public static bool Prefix(ref SpriteFont p_font, ref string p_text, ref Vector2 p_position, ref Color p_color,
            ref bool p_is_outlined)
        {
            if (!p_is_outlined)
            {
                return true;
            }

            var pref = ModEntry.Preferences;

            if (pref.IsCustomTextColor)
            {
                p_color = new Color(pref.TextRed, pref.TextGreen, pref.TextBlue, p_color.A);
            }

            if (pref.IsOutlineDisabled)
            {
                p_is_outlined = false;
                return true;
            }

            if (!pref.IsCustomOutline)
            {
                return true;
            }

            p_is_outlined = false;

            var outlineColor = new Color(pref.OutlineRed, pref.OutlineGreen, pref.OutlineBlue, p_color.A);

            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(-1f, -1f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(-1f, 0f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(-1f, 1f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(0f, -1f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(0f, 1f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(1f, -1f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(1f, 0f)), outlineColor);
            Game1.spriteBatch.DrawString(p_font, p_text, Vector2.Add(p_position, new Vector2(1f, 1f)), outlineColor);

            return true;
        }
    }
}
