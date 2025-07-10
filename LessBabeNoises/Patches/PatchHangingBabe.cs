namespace LessBabeNoises.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.HangingBabe", "MakeBT")]
    public static class PatchHangingBabe
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void Postfix(BehaviorTreeComp result)
        {
            /* Sounds, in order played, are:
                1 - babe.Mou
             */

            if (!ModEntry.MuteNewBabe)
            {
                return;
            }

            var sequencorChildren = Traverse
                .Create(result.GetRaw())
                .Field("m_root_node")
                .Field("m_children");
            var filteredNodes = sequencorChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = sequencorChildren
                .SetValue(filteredNodes.ToArray());
        }
    }
}
