namespace MoreTextOptions.Menus
{
    using JumpKing;
    using JumpKing.PauseMenu.BT;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;

    public class TextInfoExample : TextInfo
    {
        public TextInfoExample() : base("Example Text", Color.White)
        {
        }

        public override void Draw(int x, int y, bool selected)
        {
            base.Draw(x, y, selected);
            TextHelper.DrawString(Game1.instance.contentManager.font.MenuFont,
                "Example Text",
                new Vector2(x, y),
                Color.White,
                Vector2.Zero,
                true);
        }
    }
}
