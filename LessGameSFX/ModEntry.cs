namespace LessGameSFX
{
    using System.Reflection;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using Patches;
#if DEBUG
    using System.Diagnostics;
#endif

    [JumpKingMod(Identifier)]
    public static class ModEntry
    {
        private const string Identifier = "Zebra.LessGameSFX";

        /// <summary>
        ///     Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        [UsedImplicitly]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
#endif
            var harmony = new Harmony(Identifier);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///     Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        [UsedImplicitly]
        public static void OnLevelStart()
        {
            PatchParticleSpawner.MuteWaterSfx = false;

            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return;
            }

            foreach (var tag in tags)
            {
                switch (tag)
                {
                    case "MuteWaterSplash":
                        PatchParticleSpawner.MuteWaterSfx = true;
                        break;
                    case "MuteHiddenWallSFX":
                        PatchRaymanWallEntity.MuteAll();
                        break;
                }

                var match = PatchRaymanWallEntity.TagRegex.Match(tag);
                if (!match.Success)
                {
                    continue;
                }

                PatchRaymanWallEntity.MuteScreens(PatchRaymanWallEntity.GetMutedScreens(match.Value));
            }
        }
    }
}
