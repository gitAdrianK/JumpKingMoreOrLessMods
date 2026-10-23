namespace MoreEntityOptions.BehaviourTree
{
    using System.Collections.Generic;
    using BehaviorTree;
    using ErikMaths;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Util;

    public class MsPause : IBTnode
    {
        public MsPause(PauseRandom random)
        {
            this.Receiver = new List<MsResultReceiver>();

            var traverse = Traverse.Create(random);
            this.Min = traverse.Field("m_min").GetValue<float>();
            this.Max = traverse.Field("m_max").GetValue<float>();
            this.Timer = traverse.Field("m_timer").GetValue<Timer>();
        }

        public List<MsResultReceiver> Receiver { get; }

        private float Min { get; }
        private float Max { get; }
        private Timer Timer { get; set; }

        protected override void OnNewRun()
        {
            base.OnNewRun();
            this.Timer = new Timer(ErikMath.Lerpf(this.Min, this.Max, (float)Game1.random.NextDouble()));
        }

        protected override BTresult MyRun(TickData tickData)
        {
            var newResult = this.Timer.Update(tickData.delta_time) == Timer.Result.Done
                ? BTresult.Success
                : BTresult.Running;
            this.UpdateAll(newResult);
            return newResult;
        }

        public void AddReceiver(MsResultReceiver receiver) => this.Receiver.Add(receiver);

        public void UpdateAll(BTresult newResult)
        {
            foreach (var lastResult in this.Receiver)
            {
                lastResult.LastResult = newResult;
            }
        }
    }
}
