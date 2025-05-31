namespace MoreTextOptions.Menues
{
    using System.Diagnostics.CodeAnalysis;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT.Actions;
    using Microsoft.Xna.Framework;

    public class SliderOutlineBlue : ISlider
    {
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static SliderOutlineBlue Slider(object factory, GuiFormat format)
            => new SliderOutlineBlue();

        public SliderOutlineBlue() : base(ModEntry.Preferences.OutlineBlue / 255.0f)
        {
        }

        protected override void IconDraw(float p_value, int x, int y, out int new_x)
        {
            Game1.spriteBatch.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                "Blue",
                new Vector2(x, y - (ModEntry.OffsetY / 4)),
                Color.White);
            new_x = x + ModEntry.OffsetX + 5;
            Game1.spriteBatch.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                ((int)(255 * p_value)).ToString(),
                new Vector2(new_x + 65, y - (ModEntry.OffsetY / 4)),
                Color.White);
        }

        protected override void OnSliderChange(float p_value)
            => ModEntry.Preferences.OutlineBlue = (int)(255 * p_value);
    }
}
