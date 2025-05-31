namespace MoreTextOptions.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleCustomText : ITextToggle
    {
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static ToggleCustomText Toggle(object factory, GuiFormat format)
            => new ToggleCustomText();

        public ToggleCustomText() : base(ModEntry.Preferences.IsCustomTextColor)
        {
        }

        protected override string GetName() => "Custom text colour";

        protected override void OnToggle()
            => ModEntry.Preferences.IsCustomTextColor = !ModEntry.Preferences.IsCustomTextColor;
    }
}
