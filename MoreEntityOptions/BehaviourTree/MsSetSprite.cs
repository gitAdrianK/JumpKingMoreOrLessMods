namespace MoreEntityOptions.BehaviourTree
{
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Util.DrawBT;
    using JumpKing.Util.DrawBT.Util;

    public class MsSetSprite : ISpriteBTnode
    {
        public MsSetSprite(SetRandomSprite random) : base(random.SpriteEntity) =>
            this.Sprites = Traverse.Create(random).Field("m_sprites").GetValue<Sprite[]>();

        private Sprite[] Sprites { get; }

        protected override BTresult MyRun(TickData tickData) => BTresult.Success;

        public void UpdateSprite(int num) => this.SpriteEntity.SetSprite(this.Sprites[num]);
    }
}
