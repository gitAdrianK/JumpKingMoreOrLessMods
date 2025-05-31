namespace LessLocationText.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleEnterLocationText : ITextToggle
    {
        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleEnterLocationText ToggleEnter(object factory, GuiFormat format) => new ToggleEnterLocationText();

        public ToggleEnterLocationText() : base(ModEntry.Preferences.ShouldHideEnter)
        {
        }

        protected override string GetName() => "Disable enter text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideEnter = !ModEntry.Preferences.ShouldHideEnter;
    }
}
