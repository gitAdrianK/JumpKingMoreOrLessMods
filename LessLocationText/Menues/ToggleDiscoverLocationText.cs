namespace LessLocationText.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDiscoverLocationText : ITextToggle
    {
        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleDiscoverLocationText ToggleDiscover(object factory, GuiFormat format) => new ToggleDiscoverLocationText();

        public ToggleDiscoverLocationText() : base(ModEntry.Preferences.ShouldHideDiscover)
        {
        }

        protected override string GetName() => "Disable discover text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideDiscover = !ModEntry.Preferences.ShouldHideDiscover;
    }
}
