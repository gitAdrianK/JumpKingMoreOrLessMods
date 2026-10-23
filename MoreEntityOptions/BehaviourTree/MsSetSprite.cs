namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Util.DrawBT;
    using JumpKing.Util.DrawBT.Util;
    using Util;

    public class MsSetSprite : ISpriteBTnode
    {
        public MsSetSprite(SetRandomSprite random) : base(random.SpriteEntity) =>
            this.Sprites = Traverse.Create(random).Field("m_sprites").GetValue<Sprite[]>();

        private Sprite[] Sprites { get; }

        protected override BTresult MyRun(TickData tickData)
        {
            if (!Ravens.ProviderRavenDone)
            {
                return BTresult.Success;
            }

            var num = Game1.random.Next(0, this.Sprites.Length);
            this.SpriteEntity.SetSprite(this.Sprites[num]);

            return BTresult.Success;
        }

        public void UpdateSprite(int num) => this.SpriteEntity.SetSprite(this.Sprites[num]);
    }
}
