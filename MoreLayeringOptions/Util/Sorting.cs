namespace MoreLayeringOptions.Util
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using EntityComponent;
    using HarmonyLib;
    using JumpKing.Player;
    using Microsoft.Xna.Framework;

    public static class Sorting
    {
        public static void MoveBirdBeforeProps()
        {
            var entityManager = EntityManager.instance;

            var ravens = new List<Entity>();
            var others = new List<Entity>(entityManager.Entities.Count);

            foreach (var entity in entityManager.Entities)
            {
                var type = entity.GetType().ToString();
                if (type.Contains("RavenEntity"))
                {
                    ravens.Add(entity);
                    others.Clear();
                }
                else if (type.Contains("LoopingProp"))
                {
                    others.Clear();
                }
                else
                {
                    others.Add(entity);
                }
            }

            foreach (var raven in ravens)
            {
                entityManager.MoveToFront(raven);
            }

            foreach (var other in others)
            {
                entityManager.MoveToFront(other);
            }
        }

        public static void MoveWallsBeforePlayer(Match match)
        {
            var screens = new HashSet<int>();
            if (match.Groups[1].Success)
            {
                foreach (var part in match.Groups[1].Value.Split(','))
                {
                    var range = part.Trim().Split('-');
                    var start = int.Parse(range[0].Trim());
                    var end = range.Length > 1 ? int.Parse(range[1].Trim()) : start;
                    for (var i = start; i <= end; i++)
                    {
                        screens.Add(i);
                    }
                }
            }

            var playerType = typeof(PlayerEntity);
            var raymanWallType = AccessTools.TypeByName("JumpKing.Props.RaymanWall.RaymanWallEntity");
            var PositionRef =
                AccessTools.FieldRefAccess<Entity, Vector2>(
                    AccessTools.Field("JumpKing.Props.RaymanWall.RaymanWallEntity:m_position"));

            var entityManager = EntityManager.instance;

            var walls = new List<Entity>();
            var others = new List<Entity>(entityManager.Entities.Count);

            foreach (var entity in entityManager.Entities)
            {
                var type = entity.GetType();
                if (type == raymanWallType)
                {
                    if (screens.Count == 0)
                    {
                        walls.Add(entity);
                        others.Clear();
                        continue;
                    }

                    var screen = 1 + (((int)PositionRef(entity).Y - 346) / -360);
                    if (!screens.Contains(screen))
                    {
                        continue;
                    }

                    walls.Add(entity);
                    others.Clear();
                }
                else if (type == playerType)
                {
                    others.Clear();
                }
                else
                {
                    others.Add(entity);
                }
            }

            foreach (var wall in walls)
            {
                entityManager.MoveToFront(wall);
            }

            foreach (var other in others)
            {
                entityManager.MoveToFront(other);
            }
        }
    }
}
