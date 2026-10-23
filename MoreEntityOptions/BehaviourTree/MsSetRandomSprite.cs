namespace MoreEntityOptions.BehaviourTree
{
    using System.Collections.Generic;
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Util.DrawBT;
    using JumpKing.Util.DrawBT.Util;

    public class MsSetRandomSprite : ISpriteBTnode
    {
        public MsSetRandomSprite(SetRandomSprite random) : base(random.SpriteEntity)
        {
            this.Receiver = new List<MsSetSprite>();
            this.Sprites = Traverse.Create(random).Field("m_sprites").GetValue<Sprite[]>();
        }

        private List<MsSetSprite> Receiver { get; }

        private Sprite[] Sprites { get; }

        public void AddReceiver(MsSetSprite receiver) => this.Receiver.Add(receiver);

        protected override BTresult MyRun(TickData tickData)
        {
            var num = Game1.random.Next(0, this.Sprites.Length);
            this.SpriteEntity.SetSprite(this.Sprites[num]);
            foreach (var receiver in this.Receiver)
            {
                receiver.UpdateSprite(num);
            }

            return BTresult.Success;
        }
    }
}
