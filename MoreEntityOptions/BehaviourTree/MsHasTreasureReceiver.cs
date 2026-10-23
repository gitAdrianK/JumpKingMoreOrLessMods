namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;
    using EntityComponent.BT;
    using JumpKing.MiscEntities.Raven;
    using Util;

    public class MsHasTreasureReceiver : EntityBTNode
    {
        public MsHasTreasureReceiver(HasTreasure treasure) : base(treasure.game_object) { }

        public BTresult LastResult { get; set; }

        protected override BTresult MyRun(TickData tickData)
        {
            if (!Ravens.ProviderRavenDone)
            {
                return this.LastResult;
            }

            var comps = this.game_object.GetComponents();
            foreach (var comp in comps)
            {
                if (comp.GetType() != MsHasTreasure.RavenComp)
                {
                    continue;
                }

                var position = MsHasTreasure.GetCurrentPosition(comp);
                return position.treasure ? BTresult.Success : BTresult.Failure;
            }

            return BTresult.Failure;
        }
    }
}
