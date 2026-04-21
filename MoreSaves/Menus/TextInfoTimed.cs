namespace MoreSaves.Menus
{
    using JumpKing.PauseMenu.BT;
    using Microsoft.Xna.Framework;

    public class TextInfoTimed : TextInfo
    {
        private const string DefaultText = "More Saves";

        public TextInfoTimed() : base(DefaultText, Color.Yellow) { }

        private int Timer { get; set; }

        public void SetText(string text)
        {
            this.Timer = (int)(5.0d / 0.017d);
            this.Text = text;
        }

        public override void Draw(int x, int y, bool selected)
        {
            // We are abusing draw being called as an update function.
            if (this.Timer <= 0)
            {
                this.Text = DefaultText;
            }

            this.Timer--;

            base.Draw(x, y, selected);
        }
    }
}
