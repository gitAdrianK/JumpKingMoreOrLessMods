namespace MoreSaves.Patches
{
    using System;
    using HarmonyLib;
    using JumpKing.JKMemory.ManagedAssets.ThreadLube;

    [HarmonyPatch(typeof(ContentThread), "Update")]
    public static class PatchContentThread
    {

        // ReSharper disable once InconsistentNaming
        public static Exception Finalizer(Exception __exception)
            => __exception is NullReferenceException ? null : __exception;
    }
}
