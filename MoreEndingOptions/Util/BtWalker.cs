namespace MoreEndingOptions.Util
{
    using System;
    using System.Collections.Generic;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.GameManager.MultiEnding.OwlEnding;
    using JumpKing.Util;
    using JumpKing.Util.DrawBT;

    public static class BtWalker
    {
        /// <summary>
        ///     FieldRef to the root node of a <see cref="BTmanager" />.
        /// </summary>
        private static readonly AccessTools.FieldRef<BTmanager, IBTnode> RootRef =
            AccessTools.FieldRefAccess<BTmanager, IBTnode>("m_root_node");

        /// <summary>
        ///     FieldRef to the children of a <see cref="IBTcomposite" />.
        /// </summary>
        private static readonly AccessTools.FieldRef<IBTcomposite, IBTnode[]> ChildrenRef =
            AccessTools.FieldRefAccess<IBTcomposite, IBTnode[]>("m_children");

        /// <summary>
        ///     <see cref="Func{T,TResult}" /> to filter out all <see cref="PlaySFX" /> nodes.
        /// </summary>
        public static readonly Func<IBTnode, bool> PredicateMute = node => !(node is PlaySFX);

        /// <summary>
        ///     Filters all <see cref="IBTnode" /> according to a predicate from a <see cref="BehaviorTreeComp" />
        /// </summary>
        /// <param name="behaviorTree"><see cref="BehaviorTreeComp" /> to filter from.</param>
        /// <param name="predicate">Predicate to filter <see cref="IBTnode" />s.</param>
        public static void FilterRecursively(BehaviorTreeComp behaviorTree, Func<IBTnode, bool> predicate) =>
            FilterRecursively(behaviorTree.GetRaw(), predicate);

        /// <summary>
        ///     Filters all <see cref="IBTnode" /> according to a predicate from a <see cref="BTmanager" />
        /// </summary>
        /// <param name="btManager"><see cref="BTmanager" /> to filter from.</param>
        /// <param name="predicate">Predicate to filter <see cref="IBTnode" />s.</param>
        public static void FilterRecursively(BTmanager btManager, Func<IBTnode, bool> predicate)
        {
            var root = RootRef(btManager);
            if (!(root is IBTcomposite ibtComposite))
            {
                return;
            }

            FilterRecursively(ibtComposite, predicate);
        }

        /// <summary>
        ///     Filters all <see cref="IBTnode" /> according to a predicate from a <see cref="IBTcomposite" />
        /// </summary>
        /// <param name="composite"><see cref="IBTcomposite" /> to filter from.</param>
        /// <param name="predicate">Predicate to filter <see cref="IBTnode" />s.</param>
        public static void FilterRecursively(IBTcomposite composite, Func<IBTnode, bool> predicate)
        {
            var filtered = new List<IBTnode>();
            foreach (var child in composite.Children)
            {
                if (!predicate(child))
                {
                    continue;
                }

                if (child is IBTcomposite ibtComposite)
                {
                    FilterRecursively(ibtComposite, predicate);
                }

                filtered.Add(child);
            }

            ChildrenRef(composite) = filtered.ToArray();
        }

        /// <summary>
        ///     Removes the noises made by the babe in the ending.
        /// </summary>
        /// <param name="ending">The ending the babe belongs to.</param>
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

            if (ending is OwlEnding)
            {
                // Just like the babes the gargoyle exists from the start on.
                var btComp2 = Traverse
                    .Create(ending)
                    .Field("m_gargoyle")
                    .GetValue<ISpriteEntity>()
                    .GetComponent<BehaviorTreeComp>();
                FilterRecursively(btComp2, node => !(node is PlayEventSFX));
            }

            // Using Traverse here because the IEnding interface doesn't have a babe field.
            // It just happened to be that on the concrete implementations the babe field is always "m_babe".
            var btComp = Traverse
                .Create(ending)
                .Field("m_babe")
                .GetValue<ISpriteEntity>()
                .GetComponent<BehaviorTreeComp>();
            FilterRecursively(btComp, PredicateMute);
        }

        /// <summary>
        ///     Replaces the babe bt with a minimal one.
        /// </summary>
        /// <param name="ending">The ending the babe belongs to.</param>
        public static void StripBabeBt(IEnding ending)
        {
            if (ending is OwlEnding)
            {
                var gargoyle = Traverse
                    .Create(ending)
                    .Field("m_gargoyle")
                    .GetValue<ISpriteEntity>();
                var btComp2 = Traverse.Create(gargoyle.GetComponent<BehaviorTreeComp>());
                btComp2.SetValue(new BehaviorTreeComp(new SuicideNode(gargoyle)));
            }

            var babe = Traverse
                .Create(ending)
                .Field("m_babe")
                .GetValue<ISpriteEntity>();
            var btComp = Traverse.Create(babe.GetComponent<BehaviorTreeComp>());
            btComp.SetValue(new BehaviorTreeComp(new SuicideNode(babe)));
        }
    }
}
