namespace MoreEntityOptions.BehaviourTree
{
    using System.Collections.Generic;
    using BehaviorTree;
    using BehaviorTree.Util;
    using HarmonyLib;
    using JumpKing;

    public class MsRandom : IBTnode
    {
        public MsRandom(BTrandom random)
        {
            this.Receiver = new List<MsResultReceiver>();

            this.SuccessChance = Traverse.Create(random).Field("m_success_chance").GetValue<float>();
        }

        public List<MsResultReceiver> Receiver { get; }

        private float SuccessChance { get; }

        protected override BTresult MyRun(TickData tickData)
        {
            var newResult = Game1.random.NextDouble() < this.SuccessChance ? BTresult.Failure : BTresult.Success;
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
