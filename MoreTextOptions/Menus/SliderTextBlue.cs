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

    public class SliderTextBlue : ISlider
    {
        private SliderTextBlue() : base(ModEntry.Preferences.TextBlue / 255.0f)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static SliderTextBlue Slider(object factory, GuiFormat format)
            => new SliderTextBlue();

        protected override void IconDraw(float pValue, int x, int y, out int newX)
        {
            Game1.spriteBatch.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                "Blue",
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
            => ModEntry.Preferences.TextBlue = (int)(255 * pValue);
    }
}
