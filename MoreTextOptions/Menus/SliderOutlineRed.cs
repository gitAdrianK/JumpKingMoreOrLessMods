// ReSharper disable PossibleLossOfFraction

namespace MoreTextOptions.Menus
{
    using JumpKing;
    using JumpKing.PauseMenu.BT.Actions;
    using Microsoft.Xna.Framework;

    public class SliderOutlineRed : ISlider
    {
        public SliderOutlineRed() : base(ModEntry.Preferences.OutlineRed / 255.0f)
        {
        }

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
            => ModEntry.Preferences.OutlineRed = (int)(255 * pValue);
    }
}
