namespace MoreSaves.Patches
{
    using HarmonyLib;
    using JumpKing;
    using JumpKing.JKMemory.ManagedAssets.ThreadLube;

    public static class PatchContentThread
    {
        /// <summary>FieldRef of the "m_running" field.</summary>
        private static readonly AccessTools.FieldRef<ContentThread, bool> RunningRef =
            AccessTools.FieldRefAccess<ContentThread, bool>(
                AccessTools.Field("JumpKing.JKMemory.ManagedAssets.ThreadLube.ContentThread:m_running"));

        public static bool Running
        {
            get => RunningRef(Program.contentThread);
            set => RunningRef(Program.contentThread) = value;
        }
    }
}
