namespace MoreTextOptions.Menus
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleCustomOutline : ITextToggle
    {
        public ToggleCustomOutline() : base(ModEntry.Preferences.IsCustomOutline)
        {
        }

        protected override string GetName() => "Custom outline colour";

        protected override void OnToggle()
            => ModEntry.Preferences.IsCustomOutline = !ModEntry.Preferences.IsCustomOutline;
    }
}
