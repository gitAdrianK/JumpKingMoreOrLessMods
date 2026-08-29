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

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NormalEnding.EndingCherub", "MakeBT")]
    public static class PatchEndingCherub
    {
        [UsedImplicitly]
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_main_cherub.xml");
            if (File.Exists(path))
            {
                var doc = XDocument.Load(path);
                var root = doc.Root;
                if (root == null)
                {
                    throw new Exception("Tried to create custom ending but root was missing");
                }

                __result = new BehaviorTreeComp(EndingXmlParser.GetBtTree(__instance, root,
                    EndingXmlParser.Ending.MainBabe));
            }

            if (ModEntry.ShortMainBabe)
            {
                __result = new BehaviorTreeComp(new SuicideNode(__instance));
            }
        }
    }
}
