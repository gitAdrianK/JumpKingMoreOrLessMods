namespace LessLocationText.Menus
{
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleEnterLocationText : ITextToggle
    {
        private ToggleEnterLocationText() : base(ModEntry.Preferences.ShouldHideEnter)
        {
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static ToggleEnterLocationText ToggleEnter(object factory, GuiFormat format) =>
            new ToggleEnterLocationText();

        protected override string GetName() => "Disable enter text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideEnter = !ModEntry.Preferences.ShouldHideEnter;
    }
}
