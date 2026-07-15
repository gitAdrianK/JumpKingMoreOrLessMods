namespace MoreSaves.Patches
{
    using System;
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems;

    public static class PatchSkinManager
    {
        private static readonly Action<Items, bool> DelegateSetSkinEnabled =
            AccessTools.MethodDelegate<Action<Items, bool>>(
                AccessTools.Method(
                    AccessTools.TypeByName("JumpKing.Player.Skins.SkinManager"),
                    "SetSkinEnabled"));

        public static void SetSkinEnabled(Items item, bool enabled) => DelegateSetSkinEnabled(item, enabled);
    }
}
