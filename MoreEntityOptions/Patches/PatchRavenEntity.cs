namespace MoreEntityOptions.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JumpKing.MiscEntities;
    using JumpKing.MiscEntities.Raven;
    using JumpKing.Util.DrawBT;

    [HarmonyPatch("JumpKing.MiscEntities.RavenEntity", "Draw")]
    public static class PatchRavenEntity
    {
        private static readonly Type RavenComp = AccessTools.TypeByName("JumpKing.MiscEntities.Raven.RavenComp");

        private static readonly AccessTools.FieldRef<ISpriteEntity, RavenSettings> SettingsRef =
            AccessTools.FieldRefAccess<ISpriteEntity, RavenSettings>(
                AccessTools.Field("JumpKing.MiscEntities.RavenEntity:m_settings"));

        private static readonly AccessTools.FieldRef<object, RavenState> StateRef =
            AccessTools.FieldRefAccess<object, RavenState>(
                AccessTools.Field("JumpKing.MiscEntities.Raven.RavenComp:m_state"));

        public static Dictionary<string, int> HideUntil { get; set; }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            var continueLabel = il.DefineLabel();
            code.First().labels.Add(continueLabel);

            var insert = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(PatchRavenEntity), nameof(ShouldRender))),
                new CodeInstruction(
                    OpCodes.Brtrue_S, continueLabel),
                new CodeInstruction(OpCodes.Ret),
            };
            code.InsertRange(0, insert);

            return code.AsEnumerable();
        }

        public static bool ShouldRender(ISpriteEntity instance)
        {
            var settings = SettingsRef(instance);
            if (HideUntil == null || !HideUntil.TryGetValue(settings.name, out var until))
            {
                return true;
            }

            var ravenComp = instance.GetComponents().FirstOrDefault(c => c.GetType() == RavenComp);
            if (ravenComp == null)
            {
                return true;
            }

            var state = StateRef(ravenComp);
            return state.TimesFled() >= until;
        }

        public static string NameToInternal(string raven)
        {
            switch (raven)
            {
                case "Raven":
                    return "raven";
                case "WhiteRaven":
                    return "white_raven";
                case "Tsuchinoko":
                    return "tsuchinoko";
                case "Fly":
                    return "fly";
            }

            throw new ArgumentException("Unknown raven: " + raven);
        }
    }
}
