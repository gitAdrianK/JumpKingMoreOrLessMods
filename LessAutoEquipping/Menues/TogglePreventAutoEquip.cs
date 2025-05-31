namespace LessAutoEquipping.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;

    public class TogglePreventAutoEquip : ITextToggle
    {
        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static TogglePreventAutoEquip ToggleDiscover(object factory, GuiFormat format)
            => new TogglePreventAutoEquip();

        public TogglePreventAutoEquip() : base(ModEntry.Preferences.ShouldPreventAutoEquip)
        {
        }

        protected override string GetName() => "Disable auto-equip";

        protected override void OnToggle()
            => ModEntry.Preferences.ShouldPreventAutoEquip = !ModEntry.Preferences.ShouldPreventAutoEquip;
    }
}
