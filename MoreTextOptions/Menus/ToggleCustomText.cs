namespace MoreTextOptions.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleCustomText : ITextToggle
    {
        private ToggleCustomText() : base(ModEntry.Preferences.IsCustomTextColor)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleCustomText Toggle(object factory, GuiFormat format)
            => new ToggleCustomText();

        protected override string GetName() => "Custom text colour";

        protected override void OnToggle()
            => ModEntry.Preferences.IsCustomTextColor = !ModEntry.Preferences.IsCustomTextColor;
    }
}
