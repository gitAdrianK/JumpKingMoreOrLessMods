namespace LessNpcDialog.Menus
{
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleEnabled : ITextToggle
    {
        private ToggleEnabled() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [UsedImplicitly]
        public static ToggleEnabled Toggle(object factory, GuiFormat format) => new ToggleEnabled();

        protected override string GetName() => "Disable NPC dialog";

        protected override void OnToggle()
            => ModEntry.Preferences.IsEnabled = !ModEntry.Preferences.IsEnabled;
    }
}
