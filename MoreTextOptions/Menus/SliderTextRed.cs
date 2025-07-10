// ReSharper disable PossibleLossOfFraction

namespace MoreTextOptions.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;
    using Microsoft.Xna.Framework;

    public class SliderTextRed : ISlider
    {
        private SliderTextRed() : base(ModEntry.Preferences.TextRed / 255.0f)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderTextRed Slider(object factory, GuiFormat format)
            => new SliderTextRed();

        protected override void IconDraw(float pValue, int x, int y, out int newX)
        {
            Game1.spriteBatch.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                "Red",
                new Vector2(x, y - (ModEntry.OffsetY / 4)),
                Color.White);
            newX = x + ModEntry.OffsetX + 5;
            Game1.spriteBatch.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                ((int)(255 * pValue)).ToString(),
                new Vector2(newX + 65, y - (ModEntry.OffsetY / 4)),
                Color.White);
        }

        protected override void OnSliderChange(float pValue)
            => ModEntry.Preferences.TextRed = (int)(255 * pValue);
    }
}
