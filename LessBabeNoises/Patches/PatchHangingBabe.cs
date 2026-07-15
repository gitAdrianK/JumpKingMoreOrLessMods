namespace LessBabeNoises.Patches
{
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.HangingBabe", "MakeBT")]
    public static class PatchHangingBabe
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(BehaviorTreeComp __result)
        {
            /* Sounds, in order played, are:
                1 - babe.Mou
             */

            if (!ModEntry.MuteNewBabe)
            {
                return;
            }

            var sequencerChildren = Traverse
                .Create(__result.GetRaw())
                .Field("m_root_node")
                .Field("m_children");
            var filteredNodes = sequencerChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = sequencerChildren
                .SetValue(filteredNodes.ToArray());
        }
    }
}
