// ReSharper disable InconsistentNaming

namespace MoreEndingOptions.Patches
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using BehaviorTree;
    using Ending;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.GameManager.MultiEnding.OwlEnding;
    using JumpKing.Player;
    using Microsoft.Xna.Framework;

    [HarmonyPatch]
    public static class PatchOwlEnding
    {
        [HarmonyPatch(typeof(OwlEnding), "MakeBT")]
        [HarmonyPostfix]
        public static void MakeBtPostfix(ref BTmanager __result)
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

        [HarmonyPatch(typeof(OwlEnding), "CheckWin")]
        [HarmonyPostfix]
        public static void CheckWinPostfix(OwlEnding __instance, PlayerEntity p_player, ref bool __result)
        {
            if (!ModEntry.HitboxGhostBabe.HasValue)
            {
                return;
            }

            if (Camera.CurrentScreen != __instance.GetEndingScreen0())
            {
                __result = false;
                return;
            }

            var component = p_player.m_body;
            var playerHitbox = component.GetHitbox().Center;
            if (!ModEntry.HitboxGhostBabe.Value.Contains(new Vector2(playerHitbox.X,
                    playerHitbox.Y + (__instance.GetEndingScreen0() * 360))))
            {
                __result = false;
                return;
            }

            __result = component.IsOnGround;
        }
    }
}
