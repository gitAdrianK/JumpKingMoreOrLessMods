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
            this.Receiver = new List<MsPauseReceiver>();

            var traverse = Traverse.Create(random);
            this.Min = traverse.Field("m_min").GetValue<float>();
            this.Max = traverse.Field("m_max").GetValue<float>();
        }

        public List<MsPauseReceiver> Receiver { get; }

        public float Min { get; }
        public float Max { get; }
        private Timer Timer { get; set; }

        protected override void OnNewRun()
        {
            base.OnNewRun();
            this.Timer = new Timer(ErikMath.Lerpf(this.Min, this.Max, (float)Game1.random.NextDouble()));
        }

        protected override BTresult MyRun(TickData tickData)
        {
            var newResult = this.Timer.Update(tickData.delta_time * 0.5f) == Timer.Result.Done
                ? BTresult.Success
                : BTresult.Running;
            this.UpdateAll(newResult);
            return newResult;
        }

        public void AddReceiver(MsPauseReceiver receiver) => this.Receiver.Add(receiver);

        public void UpdateAll(BTresult newResult)
        {
            foreach (var receiver in this.Receiver)
            {
                receiver.LastResult = newResult;
            }
        }
    }
}
