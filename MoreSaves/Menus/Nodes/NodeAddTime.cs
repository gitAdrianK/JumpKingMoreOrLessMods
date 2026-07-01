namespace MoreSaves.Menus.Nodes
{
    using BehaviorTree;
    using JumpKing;
    using Patches;

    public class NodeAddTime : IBTnode
    {
        public const double DeltaTime = 0.017d;

        public NodeAddTime(NumberSelector hours, NumberSelector minutes, NumberSelector seconds)
        {
            this.SelectorHours = hours;
            this.SelectorMinutes = minutes;
            this.SelectorSeconds = seconds;
        }

        private NumberSelector SelectorHours { get; }
        private NumberSelector SelectorMinutes { get; }
        private NumberSelector SelectorSeconds { get; }

        protected override BTresult MyRun(TickData data)
        {
            var ticksHours = this.SelectorHours.CurrentValue * 3600 / DeltaTime;
            var ticksMinutes = this.SelectorMinutes.CurrentValue * 60 / DeltaTime;
            var ticksSeconds = this.SelectorSeconds.CurrentValue / DeltaTime;

            var snapshot = PatchAchievementManager.Snapshot;
            snapshot._ticks -= (int)(ticksHours + ticksMinutes + ticksSeconds);
            PatchAchievementManager.Snapshot = snapshot;
            PatchSaveLube.PlayerStatsAttemptSnapshot = snapshot;
            JumpGame.instance.UpdateMenu();

            return BTresult.Success;
        }
    }
}
