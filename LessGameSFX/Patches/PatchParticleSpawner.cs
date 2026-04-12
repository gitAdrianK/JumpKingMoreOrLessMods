namespace LessGameSFX.Patches
{
    using System.Reflection;
    using HarmonyLib;
    using Microsoft.Xna.Framework;

    [HarmonyPatch]
    public class PatchParticleSpawner
    {
        /// <summary>
        ///     If the water enter/exit sfx should be muted, as a side effect this also removes enter/exit particles.
        /// </summary>
        public static bool MuteWaterSfx { get; set; }

        /// <summary>
        ///     Method to be targeted, there are multiple so we specify the parameters.
        /// </summary>
        /// <returns><see cref="MethodBase" /> of the method to patch.</returns>
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("JumpKing.Particles.JumpParticleEntity+ParticleSpawner");

            return AccessTools.Method(
                type,
                "CreateWaterSplashParticle",
                new[] { typeof(Point), typeof(bool) }
            );
        }

        /// <summary>
        ///     Disable the method from running if enabled.
        /// </summary>
        /// <returns><c>true</c> if the method can continue, <c>false</c> otherwise.</returns>
        public static bool Prefix() => !MuteWaterSfx;
    }
}
