// ReSharper disable InconsistentNaming

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
    using JumpKing.Util.DrawBT;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.OwlEnding.OwlBirdEntity", "MakeBT")]
    public static class PatchOwlBirdEntity
    {
        [UsedImplicitly]
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_owl_bird.xml");
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
            }
        }
    }
}
