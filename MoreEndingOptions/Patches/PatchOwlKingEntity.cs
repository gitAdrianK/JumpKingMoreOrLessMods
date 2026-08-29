namespace MoreEndingOptions.Patches
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Ending;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Util;
    using JumpKing.Util.DrawBT;
    using Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.OwlEnding.OwlKingEntity", "MakeBT")]
    public static class PatchOwlKingEntity
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_owl_king.xml");
            if (File.Exists(path))
            {
                var doc = XDocument.Load(path);
                var root = doc.Root;
                if (root == null)
                {
                    throw new Exception("Tried to create custom ending but root was missing");
                }

                __result = new BehaviorTreeComp(EndingXmlParser.GetBtTree(__instance, root,
                    EndingXmlParser.Ending.GhostOfTheBabe));
            }

            if (ModEntry.ShortGhostBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
                return;
            }


            // ReSharper disable once InvertIf
            if (ModEntry.MuteGhostBabe)
            {
                /*
                 Sounds, in order played, are:
                 1 - audio.Plink
                 2 - babe.Pickup
                 3 - babe.Surprised

                 1 - player.Jump

                 1 - player.Splat
                 2 - babe.Scream
                 3 - babe.Kiss
                */
                BtWalker.FilterRecursively(__result, node => !(node is PlaySFX));
            }
        }
    }
}
