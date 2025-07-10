namespace LessAutoEquipping.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class TogglePreventAutoEquip : ITextToggle
    {
        private TogglePreventAutoEquip() : base(ModEntry.Preferences.ShouldPreventAutoEquip)
        {
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TogglePreventAutoEquip ToggleDiscover(object factory, GuiFormat format)
            => new TogglePreventAutoEquip();

        protected override string GetName() => "Disable auto-equip";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldPreventAutoEquip = !ModEntry.Preferences.ShouldPreventAutoEquip;
    }
}
