// ReSharper disable InconsistentNaming

namespace MoreEntityOptions.Patches
{
    using System.Linq;
    using HarmonyLib;
    using JumpKing.MiscEntities;
    using JumpKing.MiscEntities.Raven;
    using Util;

    [HarmonyPatch("JumpKing.MiscEntities.Raven.RavenComp", "OnFlee")]
    public static class PatchRavenComp
    {
        public static void Postfix(RavenState ___m_state, RavenSettings ___m_settings)
        {
            if (Ravens.RavenSyncDone)
            {
                return;
            }

            if (!Ravens.SyncedRavens.Contains(___m_settings.name))
            {
                return;
            }

            if (___m_state.is_done || ___m_state.TimesFled() > Ravens.UntilPosition)
            {
                Ravens.RavenSyncDone = true;
            }
        }
    }
}
