namespace MoreLayeringOptions
{
    using System.Text.RegularExpressions;
    using JumpKing;
    using JumpKing.Mods;
    using Util;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod("Zebra.MoreLayeringOptions")]
    public static class ModEntry
    {
        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
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

            var regexWalls = new Regex(@"MoveWallsBeforePlayer(?:=\(([\d\s,-]+)\))?");

            foreach (var tag in tags)
            {
                var matchWalls = regexWalls.Match(tag);

                if (tag == "MoveBirdBeforeProps")
                {
                    Sorting.MoveBirdBeforeProps();
                }
                else if (matchWalls.Success)
                {
                    Sorting.MoveWallsBeforePlayer(matchWalls);
                }
            }
        }
    }
}
