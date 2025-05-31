namespace LessBabeNoises
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
    using JumpKing.GameManager.MultiEnding.NormalEnding;
    using JumpKing.GameManager.MultiEnding.OwlEnding;
    using JumpKing.Mods;
    using JumpKing.Util;
    using JumpKing.Util.DrawBT;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.LessBabeNoises";
        private const string HARMONY_IDENTIFIER = IDENTIFIER + ".Harmony";

        public static bool MuteMainBabe { get; private set; } = false;
        public static bool MuteNewBabe { get; private set; } = false;
        public static bool MuteGhostBabe { get; private set; } = false;

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HARMONY_IDENTIFIER);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // Babes get created before OnLevelStart is called, so we cant rely on their MakeBT method to remove babe sounds!
            // We will have to get their BehaviorTreeComp some other way.
            // OnLevelEnd is called before the ending plays, so we reset here

            MuteMainBabe = false;
            MuteNewBabe = false;
            MuteGhostBabe = false;

            if (Game1.instance.contentManager?.level?.Info.Tags is null)
            {
                return;
            }

            var endings = Traverse.Create(Game1.instance.m_game)
                .Field("m_game_loop")
                .Field("m_ending_manager")
                .Field("m_endings")
                .GetValue<List<IEnding>>();
            foreach (var tag in Game1.instance.contentManager.level.Info.Tags)
            {
                if (tag == "MuteMainBabe")
                {
                    MuteMainBabe = true;
                    RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NormalEnding)));
                }
                else if (tag == "MuteNewBabe")
                {
                    MuteNewBabe = true;
                    RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(NewBabePlusEnding)));
                }
                else if (tag == "MuteGhostBabe")
                {
                    MuteGhostBabe = true;
                    RemoveBabeNoises(endings.Find(e => e.GetType() == typeof(OwlEnding)));
                }
            }
        }

        /// <summary>
        /// Removes the noises made by the babe in the ending.
        /// </summary>
        /// <param name="ending">The ending the babe belongs to</param>
        public static void RemoveBabeNoises(IEnding ending)
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
