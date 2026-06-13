namespace MoreTextOptions.Menus
{
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleRearrangeText : ITextToggle
    {
        public ToggleRearrangeText() : base(ModEntry.Preferences.ShouldRearrangeText)
        {
        }

        protected override string GetName() => "Rearrange text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldRearrangeText = !ModEntry.Preferences.ShouldRearrangeText;
    }
}
