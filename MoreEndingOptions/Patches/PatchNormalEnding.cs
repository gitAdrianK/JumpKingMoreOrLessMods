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
    using JumpKing.GameManager.MultiEnding.NormalEnding;
    using JumpKing.Player;
    using Microsoft.Xna.Framework;

    [HarmonyPatch]
    public static class PatchNormalEnding
    {
        [HarmonyPatch(typeof(NormalEnding), "MakeBT")]
        [HarmonyPostfix]
        public static void MakeBtPostfix(ref BTmanager __result)
        {
            var path = Path.Combine(Game1.instance.contentManager.root, "ending", "custom_main_ending.xml");
            if (File.Exists(path))
            {
                var doc = XDocument.Load(path);
                var root = doc.Root;
                if (root == null)
                {
                    throw new Exception("Tried to create custom ending but root was missing");
                }

                __result = new BTmanager(EndingXmlParser.GetBtTree(null, root, EndingXmlParser.Ending.MainBabe));
            }

            if (ModEntry.ShortMainBabe)
            {
                __result = new BTmanager(
                    new BTsequencor(new BTPlayMusic(Game1.instance.contentManager.audio.music.Ending)));
            }
        }

        [HarmonyPatch(typeof(NormalEnding), "CheckWin")]
        [HarmonyPostfix]
        public static void CheckWinPostfix(NormalEnding __instance, PlayerEntity p_player, ref bool __result)
        {
            if (!ModEntry.HitboxMainBabe.HasValue)
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
            if (!ModEntry.HitboxMainBabe.Value.Contains(new Vector2(playerHitbox.X,
                    playerHitbox.Y + (__instance.GetEndingScreen0() * 360))))
            {
                __result = false;
                return;
            }

            __result = component.IsOnGround;
        }
    }
}
