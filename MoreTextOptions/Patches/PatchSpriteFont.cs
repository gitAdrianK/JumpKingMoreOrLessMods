namespace MoreTextOptions.Patches
{
    using HarmonyLib;
    using JetBrains.Annotations;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpriteFont), nameof(SpriteFont.MeasureString), typeof(string))]
    public static class PatchSpriteFont
    {
        [UsedImplicitly]
        public static bool Prefix(ref string text)
        {
            text = ModEntry.Regex.Replace(text, string.Empty);
            return true;
        }
    }
}
