// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf

namespace MoreBlockSizes.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.API;
    using JumpKing.Level;
    using JumpKing.Level.Sampler;
    using JumpKing.Workshop;
    using Microsoft.Xna.Framework;

    [HarmonyPatch]
    public static class PatchLoadBlocksInterval
    {
        private const int Width = 60;
        private const int Height = 45;
        private static readonly FieldInfo BlockFactories = AccessTools.Field(typeof(LevelManager), "BlockFactories");
        private static readonly Color BlockCodeWind = Color.Lime;
        private static readonly Color BlockCodeSlope = Color.Red;

        public static bool CanMesh { get; set; }
        public static LevelTexture Sizes { get; set; }
        private static bool CanResize => !(Sizes is null);

        [UsedImplicitly]
        public static MethodBase TargetMethod() =>
            AccessTools.Method(
                typeof(LevelManager),
                "LoadBlocksInterval",
                new[]
                {
                    typeof(LevelTexture), typeof(Level), typeof(int), typeof(bool).MakeByRefType(),
                    typeof(TeleportLink[]).MakeByRefType(), typeof(float).MakeByRefType(),
                    typeof(bool?).MakeByRefType()
                });

        [UsedImplicitly]
        public static bool Prefix(ref IBlock[] __result, LevelTexture p_src, Level level, int p_screen,
            ref bool wind_enabled, ref TeleportLink[] teleport, ref float wind_int, ref bool? wind_direction)
        {
            wind_enabled = false;
            wind_int = 0.0f;
            wind_direction = null;
            teleport = new[] { new TeleportLink(), new TeleportLink() };

            if (CanMesh)
            {
                __result = WithMeshing(p_src, level, p_screen, ref wind_enabled, ref teleport, ref wind_int,
                    ref wind_direction);
                return false;
            }

            if (CanResize)
            {
                __result = WithCustomSizes(p_src, level, p_screen, ref wind_enabled, ref teleport, ref wind_int,
                    ref wind_direction);
                return false;
            }

            return true;
        }

        private static IBlock[] WithCustomSizes(
            LevelTexture src,
            Level level,
            int screen,
            ref bool windEnabled,
            ref TeleportLink[] teleport,
            ref float windInt,
            ref bool? windDirection)
        {
            var list = new List<IBlock>();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    var color = src.GetColor(screen, j, i);
                    if (color == BlockCodeWind)
                    {
                        windEnabled = true;
                    }

                    var blockFactory =
                        ((List<IBlockFactory>)BlockFactories.GetValue(null)).FirstOrDefault(bf =>
                            bf.CanMakeBlock(color, level));
                    if (blockFactory == null)
                    {
                        if (color.G == 0 && color.B == byte.MaxValue)
                        {
                            if (j >= 30)
                            {
                                teleport[1] = new TeleportLink(color.R);
                            }
                            else
                            {
                                teleport[0] = new TeleportLink(color.R);
                            }
                        }

                        if (color.G == byte.MaxValue && color.B == 0)
                        {
                            if (color.R < 208 && color.R > 191)
                            {
                                windInt = color.R - 191;
                            }

                            if (color.R > 207 && color.R < 224)
                            {
                                windInt = color.R - 207;
                                windDirection = true;
                            }

                            if (color.R > 223 && color.R < 240)
                            {
                                windInt = color.R - 223;
                                windDirection = false;
                            }

                            windEnabled = true;
                        }
                    }
                    else
                    {
                        var location = new Point(j * 8, (i - (screen * 45)) * 8);
                        var size = new Point(8, 8);
                        var block = blockFactory.GetBlock(
                            blockRect: new Rectangle(location, size),
                            blockCode: color,
                            level: level,
                            textureSrc: src,
                            currentScreen: screen,
                            x: j,
                            y: i);
                        if (block != null)
                        {
                            list.Add(block);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        private static IBlock[] WithMeshing(
            LevelTexture src,
            Level level,
            int screen,
            ref bool windEnabled,
            ref TeleportLink[] teleport,
            ref float windInt,
            ref bool? windDirection)
        {
            var visited = new bool[Width, Height];

            var list = new List<IBlock>();
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (visited[x, y])
                    {
                        continue;
                    }

                    var color = src.GetColor(screen, x, y);
                    if (color == BlockCodeWind)
                    {
                        windEnabled = true;
                    }

                    var blockFactory =
                        ((List<IBlockFactory>)BlockFactories.GetValue(null)).FirstOrDefault(bf =>
                            bf.CanMakeBlock(color, level));
                    if (blockFactory == null)
                    {
                        if (color.G == 0 && color.B == byte.MaxValue)
                        {
                            if (x >= 30)
                            {
                                teleport[1] = new TeleportLink(color.R);
                            }
                            else
                            {
                                teleport[0] = new TeleportLink(color.R);
                            }
                        }

                        if (color.G == byte.MaxValue && color.B == 0)
                        {
                            if (color.R < 208 && color.R > 191)
                            {
                                windInt = color.R - 191;
                            }

                            if (color.R > 207 && color.R < 224)
                            {
                                windInt = color.R - 207;
                                windDirection = true;
                            }

                            if (color.R > 223 && color.R < 240)
                            {
                                windInt = color.R - 223;
                                windDirection = false;
                            }

                            windEnabled = true;
                        }
                    }
                    else
                    {
                        Rectangle rect;
                        // Don't mesh blocks with custom sizes.
                        if (GetHitbox(screen, x, y, out var result))
                        {
                            rect = result;
                        }
                        else
                        {
                            var location = new Point(x * 8, (y - (screen * 45)) * 8);
                            // Don't mesh slopes, their type gets set later.
                            var meshed = color == BlockCodeSlope
                                ? new Point(1, 1)
                                : GetGreedyMeshedSize(visited, screen, x, y, color, src);
                            var size = new Point(8 * meshed.X, 8 * meshed.Y);
                            rect = new Rectangle(location, size);
                        }

                        var block = blockFactory.GetBlock(
                            blockRect: rect,
                            blockCode: color,
                            level: level,
                            textureSrc: src,
                            currentScreen: screen,
                            x: x,
                            y: y);
                        if (!(block is null))
                        {
                            list.Add(block);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        private static Point GetGreedyMeshedSize(bool[,] visited, int screen, int x, int y, Color color,
            LevelTexture texture)
        {
            var blockWidth = 1;
            var blockHeight = 1;

            while (x + blockWidth < Width
                   && !visited[x + blockWidth, y]
                   && color == texture.GetColor(screen, x + blockWidth, y))
            {
                visited[x + blockWidth, y] = true;
                blockWidth += 1;
            }

            var fullRowMatch = true;
            while (y + blockHeight < Height && fullRowMatch)
            {
                for (var dx = 0; dx < blockWidth; dx++)
                {
                    if (!visited[x + dx, y + blockHeight] && color == texture.GetColor(screen, x + dx, y + blockHeight))
                    {
                        continue;
                    }

                    fullRowMatch = false;
                    break;
                }

                if (!fullRowMatch)
                {
                    continue;
                }

                for (var dx = 0; dx < blockWidth; dx++)
                {
                    visited[x + dx, y + blockHeight] = true;
                }

                blockHeight += 1;
            }

            return new Point(blockWidth, blockHeight);
        }

        private static bool GetHitbox(int screen, int x, int y, out Rectangle result)
        {
            var location = new Point(x * 8, (y - (screen * 45)) * 8);
            var size = new Point(8, 8);
            if (Sizes == null)
            {
                result = new Rectangle(location, size);
                return false;
            }

            var color = Sizes.GetColor(screen, x, y);
            if (color.R > 63
                || color.G < 1
                || color.B < 1)
            {
                result = new Rectangle(location, size);
                return false;
            }

            var mod = color.R % 8;
            var div = color.R / 8;
            result = new Rectangle(
                location.X + mod,
                location.Y + div,
                Math.Min(color.G, 8 - mod),
                Math.Min(color.B, 8 - div));
            return true;
        }
    }
}
