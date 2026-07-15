namespace MoreTextOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;
    using JumpKing.Props.RattmanText;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(TargetLine), "MyRun")]
    public static class PatchTargetLine
    {
        private static readonly AccessTools.FieldRef<object, Dictionary<string, object>> DictionaryRef =
            AccessTools.FieldRefAccess<object, Dictionary<string, object>>(
                AccessTools.Field("EntityComponent.BlackBoardComp:m_values"));

        private static readonly Type TypeRattmanEntity =
            AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity");

        private static readonly AccessTools.FieldRef<object, RattmanSettings> RattmanSettingsRef =
            AccessTools.FieldRefAccess<object, RattmanSettings>(
                AccessTools.Field("JumpKing.Props.RattmanText.RattmanEntity:m_settings"));

        private static readonly Type TypeOldManEntity = AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity");

        private static readonly AccessTools.FieldRef<object, OldManSettings> OldManSettingsRef =
            AccessTools.FieldRefAccess<object, OldManSettings>(
                AccessTools.Field("JumpKing.MiscEntities.OldManEntity:m_settings"));

        private static readonly Func<OldManFont, SpriteFont> GetOldManFont =
            AccessTools.MethodDelegate<Func<OldManFont, SpriteFont>>(
                AccessTools.Method(AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity"), "GetOldManFont"));

        private static readonly Func<EntityBTNode, object> GetBlackBoard =
            AccessTools.MethodDelegate<Func<EntityBTNode, object>>(AccessTools
                .Method(typeof(TargetLine), "GetComponent")
                .MakeGenericMethod(AccessTools.TypeByName("EntityComponent.BlackBoardComp")));

        // ReSharper disable InconsistentNaming
        [UsedImplicitly]
        public static void Postfix(TargetLine __instance)
        {
            var typeInstance = __instance.game_object.GetType();
            OldManFont oldManFont;
            int width;

            if (typeInstance == TypeRattmanEntity)
            {
                var settings = RattmanSettingsRef(__instance.game_object);
                oldManFont = settings.font;
                width = settings.bubble_format.width;
            }
            else if (typeInstance == TypeOldManEntity)
            {
                var settings = OldManSettingsRef(__instance.game_object);
                oldManFont = settings.font;
                width = settings.bubble_format.width;
            }
            else
            {
                return;
            }

            var blackBoardComp = GetBlackBoard(__instance);
            var dict = DictionaryRef(blackBoardComp);

            var font = GetOldManFont(oldManFont);

            dict["BB_LINE_KEY"] =
                string.Join("", SpeechBubbleFormat.ChopString((string)dict["BB_LINE_KEY"], font, width));
        }
    }
}
