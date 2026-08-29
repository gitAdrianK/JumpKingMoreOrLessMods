namespace MoreEndingOptions
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
    using JumpKing.GameManager.MultiEnding.NormalEnding;
    using JumpKing.GameManager.MultiEnding.OwlEnding;
    using JumpKing.Mods;
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

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        [UsedImplicitly]
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
        [UsedImplicitly]
        public static void OnLevelStart()
        {
            // Babes get created before OnLevelStart is called, so we cant rely on their MakeBT method to remove babe sounds!
            // We will have to get their BehaviorTreeComp some other way.
            // OnLevelEnd is called before the ending plays, so we reset here

            MuteMainBabe = false;
            MuteNewBabe = false;
            MuteGhostBabe = false;

            ShortMainBabe = false;
            ShortNewBabe = false;
            ShortGhostBabe = false;

            var tags = Game1.instance.contentManager?.level?.Info.Tags;
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
            foreach (var tag in tags)
            {
                switch (tag)
                {
                    case "MuteMainBabe":
                        MuteMainBabe = true;
                        BtWalker.RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NormalEnding)));
                        break;
                    case "MuteNewBabe":
                        MuteNewBabe = true;
                        BtWalker.RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NewBabePlusEnding)));
                        break;
                    case "MuteGhostBabe":
                        MuteGhostBabe = true;
                        BtWalker.RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(OwlEnding)));
                        break;
                    case "ShortMainBabe":
                        ShortMainBabe = true;
                        BtWalker.StripBabeBt(endings.Find(e => e.GetType() == typeof(NormalEnding)));
                        break;
                    case "ShortNewBabe":
                        ShortNewBabe = true;
                        BtWalker.StripBabeBt(endings.Find(e => e.GetType() == typeof(NewBabePlusEnding)));
                        break;
                    case "ShortGhostBabe":
                        ShortGhostBabe = true;
                        BtWalker.StripBabeBt(endings.Find(e => e.GetType() == typeof(OwlEnding)));
                        break;
                }
            }
        }
    }
}
