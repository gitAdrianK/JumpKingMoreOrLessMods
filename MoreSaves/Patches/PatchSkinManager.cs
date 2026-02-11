namespace MoreSaves.Patches
{
    using System;
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems;

    public static class PatchSkinManager
    {
        private static readonly Action<Items, bool> DelegateSetSkinEnabled;

        static PatchSkinManager()
        {
            var typeSkinManager = AccessTools.TypeByName("JumpKing.Player.Skins.SkinManager");

            DelegateSetSkinEnabled = (Action<Items, bool>)typeSkinManager.GetMethod("SetSkinEnabled")
                .CreateDelegate(typeof(Action<Items, bool>));
        }

        public static void SetSkinEnabled(Items item, bool enabled) => DelegateSetSkinEnabled(item, enabled);
    }
}
