namespace MoreSaves.Patches
{
    using System.Diagnostics;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.TitleScreen;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;

    public class PatchGameTitleScreen
    {
        public static bool HasPatchingFailed { get; set; }

        public PatchGameTitleScreen(Harmony harmony)
        {
            _ = harmony.Patch(
                typeof(GameTitleScreen).GetMethod(nameof(GameTitleScreen.Draw)),
                postfix: new HarmonyMethod(typeof(PatchGameTitleScreen).GetMethod(nameof(DrawPatch))));
        }

        public static void DrawPatch()
        {
            if (!HasPatchingFailed)
            {
                return;
            }
            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFontSmall,
                "Automatic saving not working!",
                new Vector2(0f, 0f),
                Color.Red,
                new Vector2(0f, 0f),
                false);
        }

    }
}
