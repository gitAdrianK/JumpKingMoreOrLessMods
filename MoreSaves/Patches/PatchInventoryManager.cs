namespace MoreSaves.Patches
{
    using HarmonyLib;
    using JumpKing.MiscEntities.WorldItems.Inventory;

    public static class PatchInventoryManager
    {
        private static readonly Traverse Inventory;

        static PatchInventoryManager()
        {
            var inventoryManager =
                AccessTools.TypeByName("JumpKing.MiscEntities.WorldItems.Inventory.InventoryManager");

            Inventory = Traverse.Create(inventoryManager).Property("inventory");
        }

        public static void SetInventory(Inventory inv) => Inventory.SetValue(inv);

        public static Inventory GetInventory() => Inventory.GetValue<Inventory>();
    }
}
