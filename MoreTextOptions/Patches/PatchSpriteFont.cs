namespace MoreTextOptions.Patches
{
    using HarmonyLib;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpriteFont), nameof(SpriteFont.MeasureString), typeof(string))]
    public static class PatchSpriteFont
    {
        public static void Prefix(ref string text) => text = ModEntry.Regex.Replace(text, string.Empty);
    }
}
