namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;
    using JumpKing;
    using Util;

    public class MsRandomReceiver : IBTnode
    {
        public MsRandomReceiver(MsRandom provider) => this.SuccessChance = provider.SuccessChance;
        public BTresult LastResult { get; set; }

        public float SuccessChance { get; }

        protected override BTresult MyRun(TickData tickData)
        {
            if (Ravens.ProviderRavenDone)
            {
                return !(Game1.random.NextDouble() < this.SuccessChance) ? BTresult.Failure : BTresult.Success;
            }

            return this.LastResult;
        }
    }
}
