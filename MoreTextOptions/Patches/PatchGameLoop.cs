namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager;
    using Microsoft.Xna.Framework;
    using Util;

    [HarmonyPatch(typeof(GameLoop), nameof(GameLoop.Draw))]
    public class PatchGameLoop
    {
        private const int Buffer = 2;
        private static readonly Vector2 TimerDisplayPosition = new Vector2(12f, 8f);

        public static bool CanRearrange { get; set; }

        private static List<DrawTextSubmission> TopLeft { get; } = new List<DrawTextSubmission>();

        public static void SubmitTextDraw(DrawTextSubmission drawTextSubmission) => TopLeft.Add(drawTextSubmission);

        public static void Postfix()
        {
            var position = TimerDisplayPosition;
            foreach (var submission in TopLeft)
            {
                var font = submission.Font;
                var text = submission.Text;
                if (submission.IsOutlined)
                {
                    var outlineColor = submission.OutlineColor;
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(-1f, -1f)),
                        outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(-1f, 0f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(-1f, 1f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(0f, -1f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(0f, 1f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(1f, -1f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(1f, 0f)), outlineColor);
                    Game1.spriteBatch.DrawString(font, text, Vector2.Add(position, new Vector2(1f, 1f)), outlineColor);
                }

                Game1.spriteBatch.DrawString(submission.Font, submission.Text, position, submission.Color);
                position.Y += submission.Font.MeasureString(submission.Text).Y + Buffer;
            }

            TopLeft.Clear();
        }
    }
}
