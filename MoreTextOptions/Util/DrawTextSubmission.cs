namespace MoreTextOptions.Util
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class DrawTextSubmission
    {
        public DrawTextSubmission(SpriteFont font, string text, Color color, bool isOutlined, Color outlineColor)
        {
            this.Font = font;
            this.Text = text;
            this.Color = color;
            this.IsOutlined = isOutlined;
            this.OutlineColor = outlineColor;
        }

        public SpriteFont Font { get; }
        public string Text { get; }
        public Color Color { get; }
        public bool IsOutlined { get; }
        public Color OutlineColor { get; }
    }
}
