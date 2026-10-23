namespace MoreEntityOptions.BehaviourTree
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JumpKing.MiscEntities;
    using JumpKing.MiscEntities.Raven;

    public class MsHasTreasure : EntityBTNode
    {
        public static readonly Type RavenComp = AccessTools.TypeByName("JumpKing.MiscEntities.Raven.RavenComp");

        public static readonly MethodInfo CurrentPositionGetter =
            AccessTools.PropertyGetter(
                "JumpKing.MiscEntities.Raven.RavenComp:CurrentPosition"
            );

        public MsHasTreasure(HasTreasure treasure) : base(treasure.game_object) =>
            this.Receiver = new List<MsHasTreasureReceiver>();

        public List<MsHasTreasureReceiver> Receiver { get; }

        public static RavenPosition GetCurrentPosition(object ravenComp) =>
            (RavenPosition)CurrentPositionGetter.Invoke(ravenComp, null);

        public void AddReceiver(MsHasTreasureReceiver receiver) => this.Receiver.Add(receiver);

        public void UpdateAll(BTresult newResult)
        {
            foreach (var receiver in this.Receiver)
            {
                receiver.LastResult = newResult;
            }
        }

        protected override BTresult MyRun(TickData tickData)
        {
            var result = BTresult.Failure;
            var comps = this.game_object.GetComponents();
            foreach (var comp in comps)
            {
                if (comp.GetType() != RavenComp)
                {
                    continue;
                }

                var position = GetCurrentPosition(comp);
                result = position.treasure ? BTresult.Success : BTresult.Failure;
            }

            this.UpdateAll(result);
            return BTresult.Failure;
        }
    }
}
