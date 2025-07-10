namespace MoreTextOptions.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class ToggleCustomOutline : ITextToggle
    {
        private ToggleCustomOutline() : base(ModEntry.Preferences.IsCustomOutline)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleCustomOutline Toggle(object factory, GuiFormat format)
            => new ToggleCustomOutline();

        protected override string GetName() => "Custom outline colour";

        protected override void OnToggle()
            => ModEntry.Preferences.IsCustomOutline = !ModEntry.Preferences.IsCustomOutline;
    }
}
