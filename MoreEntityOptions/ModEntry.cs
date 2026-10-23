namespace MoreEntityOptions
{
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Mods;
    using Patches;
    using Util;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Name)]
    public static class ModEntry
    {
        private const string Name = "Zebra.MoreEntityOptions";
        private const string Harmony = Name + ".Harmony";

        //public static RavenSync RavenSync { get; set; }

        /// <summary>
        ///     Called by Jump King before the level loads.
        ///     -> OnGameStart
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
              Debugger.Launch();
#endif
            new Harmony(Harmony).PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            var regexWalls = new Regex(@"MoveWallsBeforePlayer(?:=\(([\d\s,-]+)\))?");
            var regexRavenSync =
                new Regex(
                    @"^SyncRavens=\((?<Ravens>(?:WhiteRaven|Raven|Tsuchinoko|Fly)(?:\s*,\s*(?:WhiteRaven|Raven|Tsuchinoko|Fly))+)\)$");

            foreach (var tag in tags)
            {
                var matchWalls = regexWalls.Match(tag);
                var matchRavenSync = regexRavenSync.Match(tag);

                if (tag == "MoveBirdBeforeProps")
                {
                    Sorting.MoveBirdBeforeProps();
                }
                else if (matchWalls.Success)
                {
                    Sorting.MoveWallsBeforePlayer(matchWalls);
                }
                else if (matchRavenSync.Success)
                {
                    var ravens = matchRavenSync.Groups["Ravens"].Value
                        .Split(',')
                        .Select(r => r.Trim())
                        .ToArray();
                    Ravens.SyncRavens(ravens);
                }
            }
        }

        /// <summary>
        ///     Called by Jump King when the level ends.
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd() => PatchRavenEntity.HideUntil = null;
    }
}
