namespace MoreEndingOptions.Entities
{
    using EntityComponent;
    using JumpKing;
    using JumpKing.Level;
    using Microsoft.Xna.Framework;

    public class EntityDebugHitbox : Entity
    {
        public EntityDebugHitbox(int screen, Rectangle rect)
        {
            this.Screen = screen;
            this.Hitbox = rect;
        }

        private int Screen { get; }
        private Rectangle Hitbox { get; }

        public override void Draw()
        {
            if (!LevelScreen.DrawDebug || Camera.CurrentScreenIndex1 != this.Screen)
            {
                return;
            }

            Game1.spriteBatch.Draw(Game1.instance.contentManager.Pixel.texture,
                new Rectangle(this.Hitbox.Location + Camera.Offset.ToPoint(), this.Hitbox.Size),
                new Color(Color.RoyalBlue, 0.66f));
        }
    }
}
