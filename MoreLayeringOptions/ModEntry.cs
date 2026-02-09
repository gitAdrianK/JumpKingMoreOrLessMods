namespace MoreLayeringOptions
{
    using System.Collections.Generic;
    using EntityComponent;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod("Zebra.MoreLayeringOptions")]
    [UsedImplicitly]
    public static class ModEntry
    {
        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        [UsedImplicitly]
        public static void OnLevelStart()
        {
#if DEBUG
            Debugger.Launch();
#endif

            /* The order of entities for vanilla (and presumably any map) is:
                 JumpKing.PauseMenu.MenuFactory
                 JumpKing.GameManager.MultiEnding.NormalEnding.EndingBabe
                 JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPBabeEntity
                 JumpKing.GameManager.MultiEnding.OwlEnding.OwlGargoyleEntity
                 JumpKing.GameManager.MultiEnding.OwlEnding.OwlBabeEntity
            4x   JumpKing.MiscEntities.RavenEntity
            27x  JumpKing.MiscEntities.OldManEntity
            5x   JumpKing.MiscEntities.Merchant.MerchantEntity
                 JumpKing.MiscEntities.WorldItems.Entities.BasicWorldItemEntity
                 JumpKing.MiscEntities.WorldItems.Entities.WorldItemEntityDisplay
            25x  JumpKing.MiscEntities.WorldItems.Entities.BasicWorldItemEntity
                 JumpKing.MiscEntities.WorldItems.Entities.WorldItemEntityDisplay
            324x JumpKing.Props.LoopingProp
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
                 JumpKing.Props.RaymanWall.RaymanWallEntity
                 JumpKing.Props.Achievents.AchievementTrigger
            92x  JumpKing.Props.RaymanWall.RaymanWallEntity
            5x   JumpKing.Props.LoopingProp
            4x   JumpKing.Props.Achievents.AchievementTrigger
            23x  JumpKing.Props.RattmanText.RattmanEntity
                 JumpKing.PauseMenu.MenuFactory
                 JumpKing.Player.PlayerEntity
                 JumpKing.AmbienceManager
                 JumpKing.MiscSystems.LocationText.LocationTextManager
                 JumpKing.MiscSystems.ScreenEventManager
                 JumpKing.MiscSystems.FullRunManager
                 JumpKing.MiscEntities.EarthquakeEntity
            */

            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            foreach (var tag in tags)
            {
                if (tag == "MoveBirdBeforeProps")
                {
                    MoveBirdBeforeProps();
                    break;
                }

                if (tag != "MoveWallsBeforePlayer")
                {
                    continue;
                }

                MoveWallsBeforePlayer();
                break;
            }
        }

        private static void MoveBirdBeforeProps()
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

        private static void MoveWallsBeforePlayer()
        {
            var entityManager = EntityManager.instance;

            var walls = new List<Entity>();
            var others = new List<Entity>(entityManager.Entities.Count);

            foreach (var entity in entityManager.Entities)
            {
                var type = entity.GetType().ToString();
                if (type.Contains("RaymanWallEntity"))
                {
                    walls.Add(entity);
                    others.Clear();
                }
                else if (type.Contains("PlayerEntity"))
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
