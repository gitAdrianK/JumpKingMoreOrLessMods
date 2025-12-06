namespace MoreBlockSizes.Patches
{
    using System.IO;
    using System.Linq;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Level;
    using JumpKing.Level.Sampler;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadScreens))]
    public static class PatchLoadScreens
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            var contentManager = Game1.instance.contentManager;

            var tags = contentManager.level?.Info.Tags;
            if (!(tags is null))
            {
                PatchLoadBlocksInterval.CanMesh = tags.Contains("MoreBlocksCanMesh");
            }

            // Can't use Path.Combine as the path in reload is hardcoded to be ...bin\\sizes
            //var file = Path.Combine(contentManager.root, "sizes");
            var file = contentManager.root + "\\" + "sizes";
            if (!File.Exists(file + ".xnb"))
            {
                return true;
            }

            var texture = contentManager.Load<Texture2D>(file);
            if (LevelDebugState.instance != null)
            {
                contentManager.ReloadAsset<Texture2D>("sizes", true);
            }

            var levelTexture = Game1.instance.contentManager.LevelTexture;
            if (texture.Height != levelTexture.Height || texture.Width != levelTexture.Width)
            {
                return true;
            }

            PatchLoadBlocksInterval.Sizes = LevelTexture.FromTexture(texture);
            return true;
        }

        [UsedImplicitly]
        public static void Postfix() => PatchLoadBlocksInterval.Sizes = null;
    }
}
