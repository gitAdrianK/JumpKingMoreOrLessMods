namespace MoreSaves.Menus.Models
{
    using HarmonyLib;
    using JumpKing;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using JumpKing.SaveThread;
    using Microsoft.Xna.Framework;
    using Nodes;

    public static class ModelAddTime
    {
        public static MenuSelectorClosePopup CreateAddTimePage(object factory, GuiFormat format)
        {
            if (SaveManager.instance.IsNewGame)
            {
                var emptySelector = new MenuSelectorClosePopup(format);
                emptySelector.AddChild(new TextInfo("No save active.", Color.Gray));
                emptySelector.Initialize();
                return emptySelector;
            }

            var menuFactory = Traverse.Create(factory);
            var guiFormat = menuFactory.Field("GUI_FORMAT").GetValue<GuiFormat>();
            var customFormat = guiFormat;
            customFormat.anchor = Vector2.Zero;

            var menuSelector = new MenuSelectorClosePopup(customFormat);

            menuSelector.AddChild(new TextInfo("Add Time to Save!", Color.White));
            menuSelector.AddChild(new TextInfo("Note that time is added in 17ms steps.", Color.Yellow,
                Game1.instance.contentManager.font.MenuFontSmall));

            var selectorHours = new NumberSelector("Hours");
            var selectorMinutes = new NumberSelector("Minutes");
            var selectorSeconds = new NumberSelector("Seconds");

            menuSelector.AddChild(selectorHours);
            menuSelector.AddChild(selectorMinutes);
            menuSelector.AddChild(selectorSeconds);

            menuSelector.AddChild(new TextButton("Add Time!",
                new NodeAddTime(selectorHours, selectorMinutes, selectorSeconds)));

            menuSelector.Initialize();
            return menuSelector;
        }
    }
}
