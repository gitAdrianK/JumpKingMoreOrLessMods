namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;
    using Microsoft.Xna.Framework;

    [HarmonyPatch(typeof(TextHelper), nameof(TextHelper.DrawString))]
    public static class PatchTextHelper
    {
        // ReSharper disable InconsistentNaming
        public static void Prefix(
            ref Color p_color,
            ref bool p_is_outlined)
        {
            if (!p_is_outlined)
            {
                return;
            }

            var pref = ModEntry.Preferences;
            if (pref.IsCustomTextColor)
            {
                p_color = new Color(pref.TextRed, pref.TextGreen, pref.TextBlue, p_color.A);
            }

            if (pref.IsOutlineDisabled)
            {
                p_is_outlined = false;
            }
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            var replaceIndex = -1;

            int i;
            // Find the part where we want to replace with our own IL instructions.
            for (i = 0; i < code.Count - 4; i++)
            {
                if (code[i].opcode != OpCodes.Ldloca_S
                    || code[i + 1].opcode != OpCodes.Ldc_I4
                    || code[i + 2].opcode != OpCodes.Ldc_I4
                    || code[i + 3].opcode != OpCodes.Ldc_I4
                    || code[i + 4].opcode != OpCodes.Call)
                {
                    continue;
                }

                replaceIndex = i;
                break;
            }

            if (replaceIndex == -1)
            {
                return code;
            }

            var replacement = new List<CodeInstruction>
            {
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(PatchTextHelper), nameof(GetOutlineColour))),
                new CodeInstruction(
                    OpCodes.Stloc_1),
            };
            code.RemoveRange(replaceIndex, 5);
            code.InsertRange(replaceIndex, replacement);

            return code;
        }

        public static Color GetOutlineColour()
        {
            var pref = ModEntry.Preferences;
            return pref.IsCustomOutline
                ? new Color(pref.OutlineRed,
                    pref.OutlineGreen,
                    pref.OutlineBlue,
                    255)
                : Color.Gray;
        }
    }
}
/*
 * Transpiler changes the line
 * Color color = new Color(128, 128, 128);
 * to
 * Color color = PatchTextHelper.GetOutlineColour();
 *
 * Old IL:
 * ldloca.s  V_1
 * IL_0057: stloc.1
 * IL_0054: ldc.i4    128
 * ldc.i4    128
 * ldc.i4    128
 * call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Color::.ctor(int32, int32, int32)
 *
 * New IL:
 * call      valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color [MoreTextOptions]MoreTextOptions.Patches.PatchTextHelper::GetOutlineColour()
 * stloc.1
 */
