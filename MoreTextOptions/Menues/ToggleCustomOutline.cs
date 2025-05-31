namespace MoreTextOptions.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleCustomOutline : ITextToggle
    {
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleCustomOutline Toggle(object factory, GuiFormat format)
            => new ToggleCustomOutline();

        public ToggleCustomOutline() : base(ModEntry.Preferences.IsCustomOutline)
        {
        }

        protected override string GetName() => "Custom outline colour";

        protected override void OnToggle()
            => ModEntry.Preferences.IsCustomOutline = !ModEntry.Preferences.IsCustomOutline;
    }
}
