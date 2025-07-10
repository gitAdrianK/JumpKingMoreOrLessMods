namespace MoreSaves.Menus
{
    using System.Diagnostics.CodeAnalysis;
    using BehaviorTree;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Nodes;

    /// <summary>
    ///     Custom text button that adds the "Explore" icon to the end of the button.
    ///     Basically just taken from JumpKing.PauseMenu.BT.LinkButton.
    /// </summary>
    public class ButtonTextExplorer : TextButton
    {
        private readonly Color color;

        private readonly SpriteFont font;
        private readonly string text;

        private ButtonTextExplorer(string text, IBTnode child, Color color) : base(text, child, color)
        {
            this.font = Game1.instance.contentManager.font.MenuFont;
            this.text = text;
            this.color = color;
        }

        [MainMenuItemSetting]
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ButtonTextExplorer OpenFolderExplorer(object factory, GuiFormat format)
            => new ButtonTextExplorer("Open Saves Folder", new NodeOpenFolderExplorer(), Color.Lime);

        public override void Draw(int x, int y, bool selected)
        {
            Game1.spriteBatch.DrawString(this.font, this.text, new Vector2(x, y), this.color);
            var point = this.font.MeasureString(this.text).ToPoint();
            Game1.spriteBatch.Draw(Game1.instance.contentManager.gui.Explore.texture,
                new Vector2(x + point.X + 2, y + 2f), this.color);
        }

        public override Point GetSize()
        {
            var value = this.font.MeasureString(this.text);
            var value2 = new Vector2(Game1.instance.contentManager.gui.Explore.texture.Width + 2, 0f);
            return Vector2.Add(value, value2).ToPoint();
        }
    }
}
