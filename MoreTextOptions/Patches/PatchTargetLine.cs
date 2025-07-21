// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.Props.RattmanText;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(TargetLine), "MyRun")]
    public static class PatchTargetLine
    {
        private static readonly Type TypeBlackBoardComp = AccessTools.TypeByName("EntityComponent.BlackBoardComp");

        private static readonly Type TypeRattmanEntity =
            AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity");

        private static readonly Type TypeOldManEntity = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");

        private static readonly MethodInfo GetOldManFont =
            AccessTools.Method(AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity"), "GetOldManFont");

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void Postfix(TargetLine __instance)
        {
            var typeInstance = __instance.game_object.GetType();
            OldManFont oldManFont;
            int width;

            if (typeInstance == TypeRattmanEntity)
            {
                var settings = Traverse.Create(__instance.game_object)
                    .Field("m_settings")
                    .GetValue<RattmanSettings>();
                oldManFont = settings.font;
                width = settings.bubble_format.width;
            }
            else if (typeInstance == TypeOldManEntity)
            {
                var settings = Traverse.Create(__instance.game_object)
                    .Field("m_settings")
                    .GetValue<OldManSettings>();
                oldManFont = settings.font;
                width = settings.bubble_format.width;
            }
            else
            {
                return;
            }

            var blackBoardComp = __instance.GetType()
                .GetMethod("GetComponent")
                ?.MakeGenericMethod(TypeBlackBoardComp)
                .Invoke(__instance, new object[] { });
            var dict = Traverse.Create(blackBoardComp).Field("m_values").GetValue<Dictionary<string, object>>();

            var font = (SpriteFont)GetOldManFont.Invoke(null, new object[] { oldManFont });

            dict["BB_LINE_KEY"] = string.Join("",
                SpeechBubbleFormat.ChopString((string)dict["BB_LINE_KEY"], font, width));
        }
    }
}
