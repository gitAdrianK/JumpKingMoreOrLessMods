namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;

    public class MsResultReceiver : IBTnode
    {
        public BTresult LastResult { get; set; }

        protected override BTresult MyRun(TickData tickData) => this.LastResult;
    }
}
