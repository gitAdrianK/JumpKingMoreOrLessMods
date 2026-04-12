namespace LessGameSFX.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using EntityComponent;
    using HarmonyLib;
    using JumpKing.XnaWrappers;
    using Microsoft.Xna.Framework;

    public class PatchRaymanWallEntity
    {
        // Really it doesn't mute it but sets it to null so there's nothing to play.
        // In a way it is muting it.

        /// <summary>The RaymanWallEnity type.</summary>
        private static readonly Type RaymanWall =
            AccessTools.TypeByName("JumpKing.Props.RaymanWall.RaymanWallEntity");

        /// <summary>FieldRef of the "m_appear_sfx" field.</summary>
        public static readonly AccessTools.FieldRef<object, IJKSound> AppearSfxRef =
            AccessTools.FieldRefAccess<object, IJKSound>(
                AccessTools.Field("JumpKing.Props.RaymanWall.RaymanWallEntity:m_appear_sfx"));

        /// <summary>FieldRef of the "m_position" field.</summary>
        public static readonly AccessTools.FieldRef<object, Vector2> PositionRef =
            AccessTools.FieldRefAccess<object, Vector2>(
                AccessTools.Field("JumpKing.Props.RaymanWall.RaymanWallEntity:m_position"));

        /// <summary>Regex to match the entire tag defining screen numbers.</summary>
        public static Regex TagRegex { get; } =
            new Regex(@"^MuteHiddenWallSFX=\(\s*(\d+(?:-\d+)?(?:\s*,\s*\d+(?:-\d+)?)*?)\s*\)$");

        /// <summary>Matches all numbers of ranges into their own group.</summary>
        private static Regex NumberRegex { get; } = new Regex(@"\d+(?:-\d+)?");

        /// <summary>
        ///     Mutes all RaymanWalls by setting their sound reference to null.
        /// </summary>
        public static void MuteAll()
        {
            foreach (var entity in EntityManager.instance.Entities)
            {
                if (!(entity.GetType() == RaymanWall))
                {
                    continue;
                }

                AppearSfxRef(entity) = null;
            }
        }

        /// <summary>
        ///     Creates a HashSet of Screens that RaymanWalls are supposed to be muted on. The tag contained either single
        ///     numbers that can be directly added to the mute screens or a range of the form x-y. This results in all screens
        ///     from x to y (inclusive) being muted.
        /// </summary>
        /// <param name="tagInside">The screen numbers that have been defined inside the tag.</param>
        /// <returns>A HashSet containing all screens that RaymanWalls are supposed to be muted on.</returns>
        public static HashSet<int> GetMutedScreens(string tagInside)
        {
            var screens = new HashSet<int>();
            var values = (from Match match in NumberRegex.Matches(tagInside) select match.Value).ToList();
            foreach (var value in values)
            {
                if (value.Contains("-"))
                {
                    var parts = value.Split('-');
                    var start = int.Parse(parts[0]);
                    var end = int.Parse(parts[1]);
                    if (end <= start)
                    {
                        continue;
                    }

                    foreach (var i in Enumerable.Range(start, end - start + 1))
                    {
                        _ = screens.Add(i);
                    }
                }
                else
                {
                    _ = screens.Add(int.Parse(value));
                }
            }

            return screens;
        }

        /// <summary>
        ///     Mutes RaymanWalls by setting their sound reference to null should their screen position
        ///     be inside the HashSet of screens that RaymanWalls are supposed to be muted on.
        /// </summary>
        /// <param name="screens">Screens that RaymanWalls are supposed to be muted on.</param>
        public static void MuteScreens(HashSet<int> screens)
        {
            foreach (var entity in EntityManager.instance.Entities)
            {
                if (!(entity.GetType() == RaymanWall))
                {
                    continue;
                }

                // From 346 to -13 is screen 1 => 0 to -360.
                // From -14 to -373 is screen 2 => -360 to -720.
                var screen = 1 + (((int)PositionRef(entity).Y - 346) / -360);
                if (screens.Contains(screen))
                {
                    AppearSfxRef(entity) = null;
                }
            }
        }
    }
}
