// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.Props.RattmanText;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(TargetLine), "MyRun")]
    public static class PatchTargetLine
    {
        private static readonly Type TypeBb = AccessTools.TypeByName("EntityComponent.BlackBoardComp");
        private static readonly Type TypeRattman = AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity");
        private static readonly Type TypeOldMan = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void Postfix(TargetLine __instance)
        {
            var typeInstance = __instance.game_object.GetType();
            OldManFont oldManFont;
            int width;

            if (typeInstance == TypeRattman)
            {
                var settings = Traverse.Create(__instance.game_object)
                    .Field("m_settings")
                    .GetValue<RattmanSettings>();
                oldManFont = settings.font;
                width = settings.bubble_format.width;
            }
            else if (typeInstance == TypeOldMan)
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
                ?.MakeGenericMethod(TypeBb)
                .Invoke(__instance, new object[] { });
            var dict = Traverse.Create(blackBoardComp).Field("m_values").GetValue<Dictionary<string, object>>();

            SpriteFont font;
            switch (oldManFont)
            {
                case OldManFont.Default:
                    font = Game1.instance.contentManager.font.StyleFont;
                    break;
                case OldManFont.Gargoyle:
                    font = Game1.instance.contentManager.font.GargoyleFont;
                    break;
                default:
                    throw new Exception("Unknown font choice");
            }

            dict["BB_LINE_KEY"] = string.Join("",
                SpeechBubbleFormat.ChopString((string)dict["BB_LINE_KEY"], font, width));
        }
    }
}
