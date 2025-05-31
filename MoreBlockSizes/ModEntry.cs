namespace MoreBlockSizes
{
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.Mods;

    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        private const string IDENTIFIER = "Zebra.MoreBlockSizes";
        private const string HARMONY_IDENTIFIER = IDENTIFIER + ".Harmony";

        /// <summary>
        /// Called by Jump King before the level loads.
        /// -> OnGameStart
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HARMONY_IDENTIFIER);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
