namespace LessLocationText.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleDiscoverLocationText : ITextToggle
    {
        private ToggleDiscoverLocationText() : base(ModEntry.Preferences.ShouldHideDiscover)
        {
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleDiscoverLocationText ToggleDiscover(object factory, GuiFormat format) =>
            new ToggleDiscoverLocationText();

        protected override string GetName() => "Disable discover text";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldHideDiscover = !ModEntry.Preferences.ShouldHideDiscover;
    }
}
