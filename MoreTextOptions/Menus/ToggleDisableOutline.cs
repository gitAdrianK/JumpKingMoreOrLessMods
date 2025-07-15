namespace MoreTextOptions.Menus
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDisableOutline : ITextToggle
    {
        public ToggleDisableOutline() : base(ModEntry.Preferences.IsOutlineDisabled)
        {
        }

        protected override string GetName() => "Disable text outline";

        protected override void OnToggle()
            => ModEntry.Preferences.IsOutlineDisabled = !ModEntry.Preferences.IsOutlineDisabled;
    }
}
