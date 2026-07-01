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

        public NumberSelector(string info, int maxValue = int.MaxValue, bool canLoop = false,
            NumberSelector spill = null)
        {
            this.Info = info;
            this.MaxValue = maxValue;
            this.CanLoop = canLoop;
            this.Spill = spill;
        }

        private static SpriteFont MenuFont => Game1.instance.contentManager.font.MenuFont;

        private int Height { get; } = (Game1.instance.contentManager.gui.SliderLeft.source.Height * 2) + 1;

        private string Info { get; }
        private int MaxValue { get; }
        private bool CanLoop { get; }
        private NumberSelector Spill { get; }

        public int CurrentValue { get; private set; }

        protected override BTresult MyRun(TickData data)
        {
            var padState = ControllerManager.instance.MenuController.GetPadState();
            var change = -Convert.ToInt32(padState.left) + Convert.ToInt32(padState.right);
            switch (change)
            {
                case 1:
                    this.AddOne();
                    break;
                case -1:
                    this.SubtractOne();
                    break;
                default:
                    return BTresult.Failure;
            }

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

        public void AddOne()
        {
            if (this.CurrentValue == this.MaxValue)
            {
                this.CurrentValue = 0;
                this.Spill.AddOne();
                return;
            }

            this.CurrentValue += 1;
        }

        public void SubtractOne()
        {
            if (this.CanLoop && this.CurrentValue == 0)
            {
                this.CurrentValue = this.MaxValue;
                return;
            }

            this.CurrentValue = Math.Max(0, this.CurrentValue - 1);
        }
    }
}
