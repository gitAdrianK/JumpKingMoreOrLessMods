// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Util;

    [HarmonyPatch(typeof(TextHelper), nameof(TextHelper.DrawString))]
    public static class PatchTextHelper
    {
        public static object PauseManager { get; set; }

        public static AccessTools.FieldRef<object, bool> IsPausedRef { get; set; }

        [UsedImplicitly]
        public static bool Prefix(
            SpriteFont p_font,
            string p_text,
            Vector2 p_position,
            ref Color p_color,
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

            var shouldRearrange =
                PatchGameLoop.CanRearrange &&
                IsPausedRef != null &&
                PauseManager != null &&
                !IsPausedRef(PauseManager) &&
                pref.ShouldRearrangeText &&
                p_position.X < Game1.WIDTH / 2 &&
                p_position.Y < Game1.HEIGHT / 2;

            var outlineColor = Color.Gray;
            if (pref.IsOutlineDisabled)
            {
                p_is_outlined = false;
            }
            else if (pref.IsCustomOutline)
            {
                outlineColor = new Color(pref.OutlineRed, pref.OutlineGreen, pref.OutlineBlue, 255);
            }

            if (shouldRearrange)
            {
                PatchGameLoop.SubmitTextDraw(
                    new DrawTextSubmission(
                        p_font,
                        p_text,
                        p_color,
                        p_is_outlined,
                        outlineColor));
                return false;
            }

            if (pref.IsOutlineDisabled || !pref.IsCustomOutline)
            {
                return true;
            }

            p_is_outlined = false;

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
