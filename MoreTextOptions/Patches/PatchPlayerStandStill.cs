namespace MoreTextOptions.Patches
{
    using BehaviorTree;
    using HarmonyLib;

    [HarmonyPatch("JumpKing.Props.RattmanText.RattmanEntity+PlayerStandStill", "MyRun")]
    public static class PatchPlayerStandStill
    {
        private static bool DisableStandStillRequirement { get; set; }

        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref BTresult __result)
        {
            if (DisableStandStillRequirement)
            {
                __result = BTresult.Success;
            }
        }

        public static void CheckStandStillRequirement(string[] tags)
        {
            DisableStandStillRequirement = false;

            if (tags is null)
            {
                return;
            }

            foreach (var tag in tags)
            {
                if (tag == "DisableStandStillRequirement")
                {
                    DisableStandStillRequirement = true;
                }
            }
        }
    }
}
