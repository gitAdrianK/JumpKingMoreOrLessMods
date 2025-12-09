// ReSharper disable InconsistentNaming
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace MoreBlockSizes.Patches
{
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Level;
    using Microsoft.Xna.Framework;

    [HarmonyPatch(typeof(LevelScreen), "DebugDraw")]
    public static class PatchDebugDraw
    {
        private static readonly Color NO_WIND_COLOR = Color.White * 0.2f;

        private static readonly AccessTools.FieldRef<LevelScreen, IBlock[]> HitboxesRef =
            AccessTools.FieldRefAccess<LevelScreen, IBlock[]>(
                AccessTools.Field("JumpKing.Level.LevelScreen:m_hitboxes"));

        [UsedImplicitly]
        public static bool Prefix(LevelScreen __instance)
        {
            if (!ModEntry.DrawGrouping)
            {
                return true;
            }

            var sprite = Game1.instance.contentManager.Pixel.sprite;
            var sprite2 = sprite;
            var hitboxes = HitboxesRef(__instance);
            foreach (var block in hitboxes)
            {
                var draw_dst = block.GetRect();
                draw_dst = new Rectangle(draw_dst.X, draw_dst.Y, draw_dst.Width - 1, draw_dst.Height - 1);
                Camera.TransformRect(ref draw_dst);
                var color = block is NoWindBlock
                    ? NO_WIND_COLOR
                    : block is IceBlock
                        ? Color.Cyan
                        : block is SnowBlock
                            ? Color.White
                            : block is WaterBlock
                                ? Color.Blue
                                : block is SandBlock
                                    ? new Color(255, 72, 0)
                                    : block is QuarkBlock
                                        ? new Color(182, 255, 0)
                                        : !(block is IBlockDebugColor blockDebugColor)
                                            ? Color.Red
                                            : blockDebugColor.DebugColor;
                if (block is BoxBlock)
                {
                    sprite2 = sprite;
                }
                else if (block is SlopeBlock slopeBlock)
                {
                    switch (slopeBlock.GetSlopeType())
                    {
                        case SlopeType.TopLeft:
                            sprite2 = Game1.instance.contentManager.slopeSprites.top_left;
                            break;
                        case SlopeType.TopRight:
                            sprite2 = Game1.instance.contentManager.slopeSprites.top_right;
                            break;
                        case SlopeType.BottomLeft:
                            sprite2 = Game1.instance.contentManager.slopeSprites.bottom_left;
                            break;
                        case SlopeType.BottomRight:
                            sprite2 = Game1.instance.contentManager.slopeSprites.bottom_right;
                            break;
                        case SlopeType.None:
                        default:
                            break;
                    }
                }

                Game1.spriteBatch.Draw(sprite2.texture, draw_dst, sprite2.source, color);
            }

            return false;
        }
    }
}
