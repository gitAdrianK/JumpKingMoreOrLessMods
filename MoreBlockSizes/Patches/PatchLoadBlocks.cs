namespace MoreBlockSizes.Patches
{
    using System.IO;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Level;
    using JumpKing.Level.Sampler;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadScreens))]
    public class PatchLoadBlocks
    {
        public static bool Prefix()
        {
            var contentManager = Game1.instance.contentManager;
            // Can't use Path.Combine as the path in reload in hardcoded to be ...bin\\sizes
            //var file = Path.Combine(contentManager.root, "sizes");
            var file = contentManager.root + "\\" + "sizes";
            if (!File.Exists(file + ".xnb"))
            {
                return true;
            }
            var texture = contentManager.Load<Texture2D>(file);
            if (LevelDebugState.instance != null)
            {
                contentManager.ReloadAsset<Texture2D>("sizes", required: true);
            }
            if (texture.Width != 780 || texture.Height != 585)
            {
                return true;
            }
            PatchLoadBlocksInterval.Sizes = LevelTexture.FromTexture(texture);
            return true;
        }

        public class PostfixLoadScreens
        {
            public static void Postfix() => PatchLoadBlocksInterval.Sizes = null;
        }
    }
}
