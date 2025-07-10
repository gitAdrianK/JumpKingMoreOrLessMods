namespace MoreTextOptions.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDisableOutline : ITextToggle
    {
        private ToggleDisableOutline() : base(ModEntry.Preferences.IsOutlineDisabled)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleDisableOutline ToggleOutline(object factory, GuiFormat format)
            => new ToggleDisableOutline();

        protected override string GetName() => "Disable text outline";

        protected override void OnToggle()
            => ModEntry.Preferences.IsOutlineDisabled = !ModEntry.Preferences.IsOutlineDisabled;
    }
}
