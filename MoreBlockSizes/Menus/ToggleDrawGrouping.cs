namespace MoreBlockSizes.Menus
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDrawGrouping : ITextToggle
    {
        public ToggleDrawGrouping() : base(false)
        {
        }

        protected override string GetName() => "Debug: Draw grouping";

        protected override void OnToggle()
            => ModEntry.DrawGrouping = !ModEntry.DrawGrouping;
    }
}
