namespace MoreSaves.Menus
{
    using System;
    using BehaviorTree;
    using JumpKing;
    using JumpKing.Controller;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class NumberSelector : IBTSimpleMenuItem
    {
        private const int ArrowWidth = 8;
        private const int ArrowAlign = 5;
        private const int Padding = 4;

        public NumberSelector(string info) => this.Info = info;

        private static SpriteFont MenuFont => Game1.instance.contentManager.font.MenuFont;

        private int Height { get; } = (Game1.instance.contentManager.gui.SliderLeft.source.Height * 2) + 1;

        private string Info { get; }
        public int CurrentValue { get; private set; }

        protected override BTresult MyRun(TickData data)
        {
            var padState = ControllerManager.instance.MenuController.GetPadState();
            var change = -Convert.ToInt32(padState.left) + Convert.ToInt32(padState.right);
            if (change == 0)
            {
                return BTresult.Failure;
            }

            this.CurrentValue += change;
            this.CurrentValue = Math.Max(0, this.CurrentValue);
            return BTresult.Success;
        }

        public override void Draw(int x, int y, bool selected)
        {
            y += Padding;

            Game1.instance.contentManager.gui.ArrowLeft.Draw(x, y + ArrowAlign);

            x += ArrowWidth;
            x += Padding;

            var currNumber = $"{this.CurrentValue}";
            var numberSize = MenuFont.MeasureString(currNumber);
            Game1.spriteBatch.DrawString(MenuFont, currNumber, new Vector2(x, y - (numberSize.Y / 4)), Color.White);

            x += (int)numberSize.X;
            x += Padding;

            Game1.instance.contentManager.gui.ArrowRight.Draw(x, y + ArrowAlign);

            x += ArrowWidth;
            x += Padding;

            Game1.spriteBatch.DrawString(MenuFont, this.Info, new Vector2(x, y - (numberSize.Y / 4)), Color.White);
        }

        public override Point GetSize() => MenuFont.MeasureString(this.CurrentValue.ToString()).ToPoint();
    }
}
