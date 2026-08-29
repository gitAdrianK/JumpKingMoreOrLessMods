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

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPBabeEntity", "MakeBT")]
    public static class PatchNbpBabeEntity
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_nbp_babe.xml");
            if (!File.Exists(path))
            {
                return;
            }

            var doc = XDocument.Load(path);
            var root = doc.Root;
            if (root == null)
            {
                throw new Exception("Tried to create custom ending but root was missing");
            }

            __result = new BehaviorTreeComp(EndingXmlParser.GetBtTree(__instance, root,
                EndingXmlParser.Ending.NewBabePlus));
        }
    }
}
