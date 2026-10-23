namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;
    using ErikMaths;
    using JumpKing;
    using Util;

    public class MsPauseReceiver : IBTnode
    {
        public MsPauseReceiver(MsPause provider) => this.Timer =
            new Timer(ErikMath.Lerpf(provider.Min, provider.Max, (float)Game1.random.NextDouble()));

        public BTresult LastResult { get; set; }

        public Timer Timer { get; }

        protected override BTresult MyRun(TickData tickData)
        {
            if (Ravens.RavenSyncDone)
            {
                // Completely arbitrary slowdown to prevent the raven from going hyper when the provider is done.
                return this.Timer.Update(tickData.delta_time * 0.5f) == Timer.Result.Done
                    ? BTresult.Success
                    : BTresult.Running;
            }

            return this.LastResult;
        }
    }
}
