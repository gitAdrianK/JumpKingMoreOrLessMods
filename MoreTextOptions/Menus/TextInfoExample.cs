namespace MoreTextOptions.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;

    public class TextInfoExample : TextInfo
    {
        private TextInfoExample() : base("Example Text", Color.White)
        {
        }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TextInfoExample DisplayExampleText(object factory, GuiFormat format)
            => new TextInfoExample();

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
