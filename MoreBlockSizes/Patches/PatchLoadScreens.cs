namespace MoreBlockSizes.Patches
{
    using System.IO;
    using System.Linq;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Level;
    using JumpKing.Level.Sampler;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadScreens))]
    public static class PatchLoadScreens
    {
        private static readonly AccessTools.FieldRef<JKContentManager, ContentManager> ContentManagerRef =
            AccessTools.FieldRefAccess<JKContentManager, ContentManager>(
                AccessTools.Field("JumpKing.JKContentManager:contentManager"));


        public static bool Prefix()
        {
            var contentManager = Game1.instance.contentManager;
            if (File.Exists(contentManager.root + "\\" + "level+.xnb"))
            {
                contentManager.LevelTexture = JKContentManager.SmartLoad<Texture2D>(
                    ContentManagerRef(contentManager), "level+", true);
                contentManager.ReloadAsset<Texture2D>("level+", true);
            }

            var tags = contentManager.level?.Info.Tags;
            PatchLoadBlocksInterval.CanMesh = !(tags is null) && tags.Contains("GreedyMeshBlocks");

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


        public static void Postfix() => PatchLoadBlocksInterval.Sizes = null;
    }
}
