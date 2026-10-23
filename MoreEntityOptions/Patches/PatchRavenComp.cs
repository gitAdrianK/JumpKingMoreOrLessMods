// ReSharper disable InconsistentNaming

namespace MoreEntityOptions.Patches
{
    using HarmonyLib;
    using JumpKing.MiscEntities;
    using JumpKing.MiscEntities.Raven;
    using Util;

    [HarmonyPatch("JumpKing.MiscEntities.Raven.RavenComp", "OnFlee")]
    public static class PatchRavenComp
    {
        public static void Postfix(RavenState ___m_state, RavenSettings ___m_settings)
        {
            if (___m_settings.name == Ravens.Provider && ___m_state.is_done)
            {
                Ravens.ProviderRavenDone = true;
            }
        }
    }
}
