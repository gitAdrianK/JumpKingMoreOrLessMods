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
    using Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NormalEnding.EndingKing", "MakeBT")]
    public static class PatchEndingKing
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(ISpriteEntity __instance, ref BehaviorTreeComp __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_main_king.xml");
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
                return;
            }

            if (ModEntry.MuteMainBabe)
            {
                /*
                 Sounds, in order played, are:
                 1 - player.Jump
                 2 - audio.Plink
                 3 - player.Land
                 4 - player.EndingParasol
                 5 - babe.Surprised
                 6 - babe.Surprised
                 7 - player.Jump
                 8 - babe.Scream

                 1 - babe.Mou
                */
                BtWalker.FilterRecursively(__result, BtWalker.PredicateMute);
            }
        }
    }
}
