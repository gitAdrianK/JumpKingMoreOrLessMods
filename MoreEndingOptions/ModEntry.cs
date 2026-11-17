namespace MoreEndingOptions
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.Mods;
    using Microsoft.Xna.Framework;
    using Util;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreEndingOptions";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

        public static bool MuteMainBabe { get; set; }
        public static bool MuteNewBabe { get; set; }
        public static bool MuteGhostBabe { get; set; }

        public static bool ShortMainBabe { get; set; }
        public static bool ShortNewBabe { get; set; }
        public static bool ShortGhostBabe { get; set; }

        public static Rectangle? HitboxMainBabe { get; set; }
        public static Rectangle? HitboxNewBabe { get; set; }
        public static Rectangle? HitboxGhostBabe { get; set; }

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
#endif
            var harmony = new Harmony(HarmonyIdentifier);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // Babes get created before OnLevelStart is called, so we cant rely on their MakeBT method to remove babe sounds!
            // We will have to get their BehaviorTreeComp some other way.
            // OnLevelEnd is called before the ending plays, so we reset here

            ResetValues();

            var contentManager = Game1.instance.contentManager;
            var tags = contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            // This happens once, so I'll allow Traverse.
            var endings = Traverse.Create(Game1.instance.m_game)
                .Field("m_game_loop")
                .Field("m_ending_manager")
                .Field("m_endings")
                .GetValue<List<IEnding>>();
            var mainEnding = endings.Find(e => e.GetBabeType() == EndingType.Normal);
            var newEnding = endings.Find(e => e.GetBabeType() == EndingType.NewBabePlus);
            var ghostEnding = endings.Find(e => e.GetBabeType() == EndingType.Ghost);

            foreach (var tag in tags)
            {
                if (Tags.HandleSimpleTags(tag, mainEnding, newEnding, ghostEnding))
                {
                    continue;
                }

                if (Tags.HandleHitboxTag(tag, mainEnding, newEnding, ghostEnding))
                {
                    continue;
                }

                if (Tags.HandleKingPositionTag(tag, mainEnding, newEnding, ghostEnding))
                {
                    continue;
                }

                if (Tags.HandleBabePositionTag(tag, mainEnding, newEnding, ghostEnding))
                {
                    continue;
                }

                _ = Tags.HandleGargoylePositionTag(tag, ghostEnding);
            }
        }

        private static void ResetValues()
        {
            MuteMainBabe = false;
            MuteNewBabe = false;
            MuteGhostBabe = false;

            ShortMainBabe = false;
            ShortNewBabe = false;
            ShortGhostBabe = false;

            HitboxMainBabe = null;
            HitboxNewBabe = null;
            HitboxGhostBabe = null;
        }
    }
}
