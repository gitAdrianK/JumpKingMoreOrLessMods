namespace LessNpcDialog.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleEnabled : ITextToggle
    {
        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleEnabled Toggle(object factory, GuiFormat format) => new ToggleEnabled();

        public ToggleEnabled() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Disable NPC dialog";

        protected override void OnToggle()
            => ModEntry.Preferences.IsEnabled = !ModEntry.Preferences.IsEnabled;
    }
}
