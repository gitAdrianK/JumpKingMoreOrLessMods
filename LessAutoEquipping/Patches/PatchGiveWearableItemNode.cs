namespace LessAutoEquipping.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JumpKing.GameManager.MultiEnding;

    [HarmonyPatch(typeof(GiveWearableItemNode), "MyRun")]
    public class PatchGiveWearableItemNode
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            var insertionIndex = -1;
            var continueFound = false;
            var continueLabel = il.DefineLabel();
            var enableSkin = AccessTools.Method(
                AccessTools.TypeByName("JumpKing.Player.Skins.SkinManager"),
                "EnableSkin");

            int i;
            // Find the first part, that is where we want to insert out own IL instructions.
            for (i = 0; i < code.Count - 2; i++)
            {
                if (code[i].opcode == OpCodes.Ldarg_0
                    && code[i + 1].opcode == OpCodes.Ldfld
                    && code[i + 2].opcode == OpCodes.Call
                    && (code[i + 2].operand as MethodInfo) == enableSkin)
                {
                    insertionIndex = i;
                    break;
                }
            }
            // Find the second part, that is where we want to jump to in case of auto equipping being disabled.
            for (; i < code.Count - 1; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_I4_1 && code[i + 1].opcode == OpCodes.Ret)
                {
                    continueFound = true;
                    code[i].labels.Add(continueLabel);
                    break;
                }
            }

            if (insertionIndex == -1 || !continueFound)
            {
                return code.AsEnumerable();
            }

            var insert = new List<CodeInstruction>
            {
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.PropertyGetter(typeof(ModEntry), nameof(ModEntry.Preferences))),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(Preferences), nameof(Preferences.ShouldPreventAutoEquip))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
            };
            code.InsertRange(insertionIndex, insert);

            return code.AsEnumerable();
        }
    }

    // The original IL instructions of MyRun.
    // AddItemOnce method call.
    // /* 0x0001EC4C 02           */ IL_0000: ldarg.0
    // /* 0x0001EC4D 7BB7040004   */ IL_0001: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.GameManager.MultiEnding.GiveWearableItemNode::m_item
    // /* 0x0001EC52 28E4050006   */ IL_0006: call      void JumpKing.MiscEntities.WorldItems.Inventory.InventoryManager::AddItemOnce(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // EnableSkin method call.
    // /* 0x0001EC57 02           */ IL_000B: ldarg.0
    // /* 0x0001EC58 7BB7040004   */ IL_000C: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.GameManager.MultiEnding.GiveWearableItemNode::m_item
    // /* 0x0001EC5D 2812030006   */ IL_0011: call      void JumpKing.Player.Skins.SkinManager::EnableSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // Return.
    // /* 0x0001EC62 17           */ IL_0016: ldc.i4.1
    // /* 0x0001EC63 2A           */ IL_0017: ret

    // Modified method to only equip the item if auto equipping is disabled.
    // OLD: AddItemOnce method call.
    // /* (16,4)-(16,46) main.cs */
    // /* 0x00000000 02           */ IL_0000: ldarg.0
    // /* 0x00000001 7BB7040004   */ IL_0001: ldfld         valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.GameManager.MultiEnding.GiveWearableItemNode::m_item
    // /* 0x00000006 28E4050006   */ IL_0006: call          void JumpKing.MiscEntities.WorldItems.Inventory.InventoryManager::AddItemOnce(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // NEW: Check for ShouldPreventAutoEquip being enabled.
    // /* (17,4)-(17,52) main.cs */
    // /* 0x0000000B 28????????   */ IL_000B: call          class [LessAutoEquipping]LessAutoEquipping.Preferences[LessAutoEquipping] LessAutoEquipping.ModEntry::get_Preferences()
    // /* 0x00000010 6F????????   */ IL_0010: callvirt      instance bool [LessAutoEquipping] LessAutoEquipping.Preferences::get_ShouldPreventAutoEquip()
    // /* 0x00000015 2D0B         */ IL_0015: brtrue.s      IL_0022
    //
    // OLD: EnableSkin method call.
    // /* (19,5)-(19,41) main.cs */
    // /* 0x00000017 02           */ IL_0017: ldarg.0
    // /* 0x00000018 7BB7040004   */ IL_0018: ldfld         valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.GameManager.MultiEnding.GiveWearableItemNode::m_item
    // /* 0x0000001D 2812030006   */ IL_001D: call          void JumpKing.Player.Skins.SkinManager::EnableSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // OLD: Return.
    // /* (21,4)-(21,28) main.cs */
    // /* 0x00000022 17           */ IL_0022: ldc.i4.1
    // /* 0x00000023 2A           */ IL_0023: ret
}
