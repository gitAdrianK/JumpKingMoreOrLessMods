namespace MoreEndingOptions.Patches
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using BehaviorTree;
    using Ending;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding.OwlEnding;

    [HarmonyPatch(typeof(OwlEnding), "MakeBT")]
    public static class PatchOwlEnding
    {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref BTmanager __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_owl_ending.xml");
            if (File.Exists(path))
            {
                var doc = XDocument.Load(path);
                var root = doc.Root;
                if (root == null)
                {
                    throw new Exception("Tried to create custom ending but root was missing");
                }

                __result = new BTmanager(EndingXmlParser.GetBtTree(null, root, EndingXmlParser.Ending.GhostOfTheBabe));
            }

            if (ModEntry.ShortGhostBabe)
            {
                __result = new BTmanager(
                    new BTsequencor(new BTPlayMusic(Game1.instance.contentManager.audio.music.Ending3)));
            }
        }
    }
}
