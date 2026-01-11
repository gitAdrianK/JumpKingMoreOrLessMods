namespace LessAutoEquipping.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.Merchant;

    [HarmonyPatch("JumpKing.MiscEntities.Merchant.MerchantComp", "OnSell")]
    public static class PatchMerchantComp
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            var insertionIndex = -1;
            var continueFound = false;
            var continueLabel = il.DefineLabel();
            var enableSkin = AccessTools.Method(
                AccessTools.TypeByName("JumpKing.Player.Skins.SkinManager"),
                "EnableSkin");
            var currencyType = AccessTools.Field(typeof(MerchantSettings), nameof(MerchantSettings.currency_type));

            int i;
            // Find the first part, that is where we want to insert out own IL instructions.
            for (i = 0; i < code.Count - 3; i++)
            {
                if (code[i].opcode != OpCodes.Ldarg_0
                    || code[i + 1].opcode != OpCodes.Ldflda
                    || code[i + 2].opcode != OpCodes.Ldfld
                    || code[i + 3].opcode != OpCodes.Call
                    || code[i + 3].operand as MethodInfo != enableSkin)
                {
                    continue;
                }

                insertionIndex = i;
                break;
            }

            // Find the second part, that is where we want to jump to in case of auto equipping being disabled.
            for (; i < code.Count - 2; i++)
            {
                if (code[i].opcode != OpCodes.Ldarg_0
                    || code[i + 1].opcode != OpCodes.Ldflda
                    || code[i + 2].opcode != OpCodes.Ldfld
                    || code[i + 2].operand as FieldInfo != currencyType)
                {
                    continue;
                }

                continueFound = true;
                code[i].labels.Add(continueLabel);
                break;
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

    // The original IL instructions of OnSell.
    // .
    // .
    // .
    // Check IsSkin.
    // /* 0x0001852A 02           */ IL_0032: ldarg.0
    // /* 0x0001852B 7CDA030004   */ IL_0033: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00018530 7BEA030004   */ IL_0038: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::sale_item
    // /* 0x00018535 2816030006   */ IL_003D: call      bool JumpKing.Player.Skins.SkinManager::IsSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    // /* 0x0001853A 2C10         */ IL_0042: brfalse.s IL_0054
    //
    // Call EnableSkin.
    // /* 0x0001853C 02           */ IL_0044: ldarg.0
    // /* 0x0001853D 7CDA030004   */ IL_0045: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00018542 7BEA030004   */ IL_004A: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::sale_item
    // /* 0x00018547 2812030006   */ IL_004F: call      void JumpKing.Player.Skins.SkinManager::EnableSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // Remove the required currency to buy the item.
    // /* 0x0001854C 02           */ IL_0054: ldarg.0
    // /* 0x0001854D 7CDA030004   */ IL_0055: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00018552 7BEB030004   */ IL_005A: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::currency_type
    // /* 0x00018557 02           */ IL_005F: ldarg.0
    // /* 0x00018558 7CD8030004   */ IL_0060: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantState JumpKing.MiscEntities.Merchant.MerchantComp::m_state
    // /* 0x0001855D 7BE7030004   */ IL_0065: ldfld     int32 JumpKing.MiscEntities.Merchant.MerchantState::required_gold
    // /* 0x00018562 28E8050006   */ IL_006A: call      void JumpKing.MiscEntities.WorldItems.Inventory.InventoryManager::RemoveItems(valuetype JumpKing.MiscEntities.WorldItems.Items, int32)
    // .
    // .
    // .

    // Modified method to only equip the item if auto equipping is disabled.
    // .
    // .
    // .
    // OLD: Check IsSkin.
    // /* 0x00000032 02           */ IL_0032: ldarg.0
    // /* 0x00000033 7CDA030004   */ IL_0033: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00000038 7BEA030004   */ IL_0038: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::sale_item
    // /* 0x0000003D 2816030006   */ IL_003D: call      bool JumpKing.Player.Skins.SkinManager::IsSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    // /* 0x00000042 2C1C         */ IL_0042: brfalse.s IL_0060
    //
    // NEW: Check for ShouldPreventAutoEquip being enabled.
    // /* 0x00000044 28????????   */ IL_0044: call      class [LessAutoEquipping]LessAutoEquipping.Preferences [LessAutoEquipping]LessAutoEquipping.ModEntry::get_Preferences()
    // /* 0x00000049 6F????????   */ IL_0049: callvirt  instance bool [LessAutoEquipping]LessAutoEquipping.Preferences::get_ShouldPreventAutoEquip()
    // /* 0x0000004E 2D10         */ IL_004E: brtrue.s  IL_0060
    //
    // OLD: EnableSkin method call.
    // /* (24,5)-(24,55) main.cs */
    // /* 0x00000050 02           */ IL_0050: ldarg.0
    // /* 0x00000051 7CDA030004   */ IL_0051: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00000056 7BEA030004   */ IL_0056: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::sale_item
    // /* 0x0000005B 2812030006   */ IL_005B: call      void JumpKing.Player.Skins.SkinManager::EnableSkin(valuetype JumpKing.MiscEntities.WorldItems.Items)
    //
    // OLD: Remove the required currency to buy the item.
    // /* (26,4)-(26,92) main.cs */
    // /* 0x00000060 02           */ IL_0060: ldarg.0
    // /* 0x00000061 7CDA030004   */ IL_0061: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantSettings JumpKing.MiscEntities.Merchant.MerchantComp::m_settings
    // /* 0x00000066 7BEB030004   */ IL_0066: ldfld     valuetype JumpKing.MiscEntities.WorldItems.Items JumpKing.MiscEntities.Merchant.MerchantSettings::currency_type
    // /* 0x0000006B 02           */ IL_006B: ldarg.0
    // /* 0x0000006C 7CD8030004   */ IL_006C: ldflda    valuetype JumpKing.MiscEntities.Merchant.MerchantState JumpKing.MiscEntities.Merchant.MerchantComp::m_state
    // /* 0x00000071 7BE7030004   */ IL_0071: ldfld     int32 JumpKing.MiscEntities.Merchant.MerchantState::required_gold
    // /* 0x00000076 28E8050006   */ IL_0076: call      void JumpKing.MiscEntities.WorldItems.Inventory.InventoryManager::RemoveItems(valuetype JumpKing.MiscEntities.WorldItems.Items, int32)
    // .
    // .
    // .
}
