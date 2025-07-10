namespace MoreBlockSizes
{
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Mods;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.MoreBlockSizes";
        private const string HarmonyIdentifier = Identifier + ".Harmony";

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
