namespace MoreBlockSizes
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using Menus;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreBlockSizes";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

        public static bool DrawGrouping { get; set; }

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static ToggleDrawGrouping CustomText(object factory, GuiFormat format)
            => new ToggleDrawGrouping();

        /// <summary>
        ///     Called by Jump King before the level loads.
        ///     -> OnGameStart
        /// </summary>
        [BeforeLevelLoad]
        [UsedImplicitly]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HarmonyIdentifier);
#if DEBUG
            Debugger.Launch();
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
