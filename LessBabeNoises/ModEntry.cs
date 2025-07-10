namespace LessBabeNoises
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
    using JumpKing.GameManager.MultiEnding.NormalEnding;
    using JumpKing.GameManager.MultiEnding.OwlEnding;
    using JumpKing.Mods;
    using JumpKing.Util;
    using JumpKing.Util.DrawBT;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.LessBabeNoises";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

        public static bool MuteMainBabe { get; private set; }
        public static bool MuteNewBabe { get; private set; }
        public static bool MuteGhostBabe { get; private set; }

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        [UsedImplicitly]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HarmonyIdentifier);
#if DEBUG
            Debugger.Launch();
#endif
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

            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

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
                        RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NormalEnding)));
                        break;
                    case "MuteNewBabe":
                        MuteNewBabe = true;
                        RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NewBabePlusEnding)));
                        break;
                    case "MuteGhostBabe":
                        MuteGhostBabe = true;
                        RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(OwlEnding)));
                        break;
                }
            }
        }

        /// <summary>
        ///     Removes the noises made by the babe in the ending.
        /// </summary>
        /// <param name="ending">The ending the babe belongs to</param>
        private static void RemoveBabeNoises(IEnding ending)
        {
            /* Sounds, in order played, are:
             * Main Babe
             * 1 - babe.Jump
             * 2 - player.Land
             * 3 - babe.Kiss
             * 4 - babe.Pickup
             *
             * New Babe
             * 1 - babe.Jump
             * 2 - babe.Kiss
             * 3 - babe.Mou
             * 4 - audio.Plink
             * 5 - babe.Pickup
             *
             * Ghost Babe
             * 1 - babe.Kiss
             * 2 - babe.Jump
             * 3 - babe.Pickup
             */

            var btManager = Traverse
                .Create(ending)
                .Field("m_babe")
                .GetValue<ISpriteEntity>()
                .GetComponent<BehaviorTreeComp>()
                .GetRaw();
            var btSequencor = Traverse
                .Create(btManager)
                .Field("m_root_node")
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var traverseChildren = Traverse
                .Create(btSequencor)
                .Field("m_children");
            var filteredNodes = traverseChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = traverseChildren
                .SetValue(filteredNodes.ToArray());
        }
    }
}
