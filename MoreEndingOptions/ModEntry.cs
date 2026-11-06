namespace MoreEndingOptions
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Entities;
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

        public static bool MuteMainBabe { get; private set; }
        public static bool MuteNewBabe { get; private set; }
        public static bool MuteGhostBabe { get; private set; }

        public static bool ShortMainBabe { get; private set; }
        public static bool ShortNewBabe { get; private set; }
        public static bool ShortGhostBabe { get; private set; }

        public static Rectangle? HitboxMainBabe { get; private set; }
        public static Rectangle? HitboxNewBabe { get; private set; }
        public static Rectangle? HitboxGhostBabe { get; private set; }

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
                if (HandleSimpleTags(tag, mainEnding, newEnding, ghostEnding))
                {
                    continue;
                }

                _ = HandleHitboxTag(tag, mainEnding, newEnding, ghostEnding);
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

        private static bool HandleSimpleTags(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            switch (tag)
            {
                case "MuteMainBabe":
                    MuteMainBabe = true;
                    BtWalker.RemoveBabeNoises(mainEnding);
                    return true;
                case "MuteNewBabe":
                    MuteNewBabe = true;
                    BtWalker.RemoveBabeNoises(newEnding);
                    return true;
                case "MuteGhostBabe":
                    MuteGhostBabe = true;
                    BtWalker.RemoveBabeNoises(ghostEnding);
                    return true;
                case "ShortMainBabe":
                    ShortMainBabe = true;
                    BtWalker.StripBabeBt(mainEnding);
                    return true;
                case "ShortNewBabe":
                    ShortNewBabe = true;
                    BtWalker.StripBabeBt(newEnding);
                    return true;
                case "ShortGhostBabe":
                    ShortGhostBabe = true;
                    BtWalker.StripBabeBt(ghostEnding);
                    return true;
            }

            return false;
        }

        private static bool HandleHitboxTag(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            var hitboxRegex =
                new Regex(@"^(Main|New|Ghost)BabeHitbox=\((\d+),\s*(\d+),\s*([1-9]\d*),\s*([1-9]\d*)\)$");
            var hitboxMatch = hitboxRegex.Match(tag);
            if (!hitboxMatch.Success)
            {
                return false;
            }

            var x = int.Parse(hitboxMatch.Groups[2].Value);
            var y = int.Parse(hitboxMatch.Groups[3].Value);
            var width = int.Parse(hitboxMatch.Groups[4].Value);
            var height = int.Parse(hitboxMatch.Groups[5].Value);
            var hitbox = new Rectangle(x, y, width, height);

            var screen = -1;
            switch (hitboxMatch.Groups[1].Value)
            {
                case "Main":
                    HitboxMainBabe = hitbox;
                    screen = mainEnding.GetEndingScreen0();
                    break;
                case "New":
                    HitboxNewBabe = hitbox;
                    screen = newEnding.GetEndingScreen0();
                    break;
                case "Ghost":
                    HitboxGhostBabe = hitbox;
                    screen = ghostEnding.GetEndingScreen0();
                    break;
            }

            _ = new EntityDebugHitbox(screen + 1, hitbox);
            return true;
        }
    }
}
