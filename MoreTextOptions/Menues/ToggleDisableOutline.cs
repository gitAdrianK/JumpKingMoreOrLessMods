namespace MoreTextOptions.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDisableOutline : ITextToggle
    {
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleDisableOutline ToggleOutline(object factory, GuiFormat format)
            => new ToggleDisableOutline();

        public ToggleDisableOutline() : base(ModEntry.Preferences.IsOutlineDisabled)
        {
        }

        protected override string GetName() => "Disable text outline";

        protected override void OnToggle()
            => ModEntry.Preferences.IsOutlineDisabled = !ModEntry.Preferences.IsOutlineDisabled;
    }
}
