namespace MoreTextOptions.Patches
{
    using HarmonyLib;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpriteFont), nameof(SpriteFont.MeasureString), typeof(string))]
    public static class PatchSpriteFont
    {
        public static bool Prefix(ref string text)
        {
            text = ModEntry.REGEX.Replace(text, string.Empty);
            return true;
        }
    }
}
