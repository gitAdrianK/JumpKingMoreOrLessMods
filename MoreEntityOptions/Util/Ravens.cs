namespace MoreEntityOptions.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using BehaviorTree;
    using BehaviorTree.Util;
    using BehaviourTree;
    using EntityComponent;
    using EntityComponent.BT;
    using HarmonyLib;
    using JumpKing.MiscEntities;
    using JumpKing.MiscEntities.Raven;
    using JumpKing.Util;
    using JumpKing.Util.DrawBT;
    using JumpKing.Util.DrawBT.Util;

    public static class Ravens
    {
        public static string[] SyncedRavens { get; set; }
        public static bool RavenSyncDone { get; set; }
        public static int UntilPosition { get; set; }

        public static void SyncRavens(string[] ravens, int untilPosition)
        {
            RavenSyncDone = false;
            UntilPosition = untilPosition;

            var syncedRavens = FindSyncedRavens(ravens, out var internalNames);
            SyncedRavens = internalNames;

            var isMaster = true;
            var provider = new RavenRandomNodes();
            foreach (var syncedRaven in syncedRavens)
            {
                ReplaceRandomNodes(syncedRaven, isMaster, provider);
                isMaster = false;
            }
        }

        private static ISpriteEntity[] FindSyncedRavens(string[] ravens, out string[] names)
        {
            var typeRaven = AccessTools.TypeByName("JumpKing.MiscEntities.RavenEntity");
            var settingsRef =
                AccessTools.FieldRefAccess<Entity, RavenSettings>(
                    AccessTools.Field("JumpKing.MiscEntities.RavenEntity:m_settings"));

            ISpriteEntity raven = null;
            ISpriteEntity whiteRaven = null;
            ISpriteEntity tsuchinoko = null;
            ISpriteEntity fly = null;
            var internalNames = new List<string>();

            foreach (var entity in EntityManager.instance.Entities)
            {
                if (entity.GetType() != typeRaven)
                {
                    continue;
                }

                var name = settingsRef(entity).name;
                switch (name)
                {
                    case "raven" when ravens.Contains("Raven"):
                        raven = (ISpriteEntity)entity;
                        internalNames.Add(name);
                        break;
                    case "white_raven" when ravens.Contains("WhiteRaven"):
                        whiteRaven = (ISpriteEntity)entity;
                        internalNames.Add(name);
                        break;
                    case "tsuchinoko" when ravens.Contains("Tsuchinoko"):
                        tsuchinoko = (ISpriteEntity)entity;
                        internalNames.Add(name);
                        break;
                    case "fly" when ravens.Contains("Fly"):
                        fly = (ISpriteEntity)entity;
                        internalNames.Add(name);
                        break;
                }
            }

            names = internalNames.ToArray();
            return new[] { raven, whiteRaven, tsuchinoko, fly }
                .Where(r => r != null)
                .ToArray();
        }

        private static void ReplaceRandomNodes(ISpriteEntity raven, bool isMaster, RavenRandomNodes providers)
        {
            if (raven == null)
            {
                return;
            }

            var manager = raven.GetComponent<BehaviorTreeComp>().GetRaw();
            ReplaceRandomNodes(manager, isMaster, providers);
        }

        private static void ReplaceRandomNodes(BTmanager manager, bool isMaster, RavenRandomNodes providers)
        {
            var root = Traverse.Create(manager).Field("m_root_node").GetValue<IBTnode>();
            ReplaceRandomNodes(root, isMaster, providers);
        }

        private static void ReplaceRandomNodes(IBTnode node, bool isMaster, RavenRandomNodes providers)
        {
            if (!(node is IBTcomposite composite))
            {
                return;
            }

            var children = composite.Children;
            for (var i = 0; i < children.Length; i++)
            {
                var child = children[i];
                switch (child)
                {
                    case IBTcomposite _:
                        ReplaceRandomNodes(child, isMaster, providers);
                        break;
                    case BTrandom random:
                        if (isMaster)
                        {
                            var msRandom = new MsRandom(random);
                            providers.AddNode(msRandom);
                            children[i] = msRandom;
                        }
                        else
                        {
                            var provider = (MsRandom)providers.GetNode();
                            var receiver = new MsRandomReceiver(provider);
                            provider.AddReceiver(receiver);
                            children[i] = receiver;
                        }

                        break;
                    case PauseRandom pause:
                        if (isMaster)
                        {
                            var msPause = new MsPause(pause);
                            providers.AddNode(msPause);
                            children[i] = msPause;
                        }
                        else
                        {
                            var provider = (MsPause)providers.GetNode();
                            var receiver = new MsPauseReceiver(provider);
                            provider.AddReceiver(receiver);
                            children[i] = receiver;
                        }

                        break;
                    case SetRandomSprite sprite:
                        if (isMaster)
                        {
                            var msSprite = new MsSetRandomSprite(sprite);
                            providers.AddNode(msSprite);
                            children[i] = msSprite;
                        }
                        else
                        {
                            var provider = (MsSetRandomSprite)providers.GetNode();
                            var receiver = new MsSetSprite(sprite);
                            provider.AddReceiver(receiver);
                            children[i] = receiver;
                        }

                        break;
                    case HasTreasure treasure:
                        if (isMaster)
                        {
                            var msTreasure = new MsHasTreasure(treasure);
                            providers.AddNode(msTreasure);
                            children[i] = msTreasure;
                        }
                        else
                        {
                            var provider = (MsHasTreasure)providers.GetNode();
                            var receiver = new MsHasTreasureReceiver(treasure);
                            provider.AddReceiver(receiver);
                            children[i] = receiver;
                        }

                        break;
                }
            }
        }

        private class RavenRandomNodes
        {
            private readonly List<IBTnode> nodes = new List<IBTnode>();
            // A raven should always have the same order of bTRandom/PauseRandom/SetRandomSprite.
            // This certainly is a way to do it.

            private int index;

            public void AddNode(IBTnode node) => this.nodes.Add(node);

            public IBTnode GetNode()
            {
                var node = this.nodes[this.index];
                this.index = (this.index + 1) % this.nodes.Count;
                return node;
            }
        }
    }
}
