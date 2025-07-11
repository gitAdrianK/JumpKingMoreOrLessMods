// ReSharper disable InconsistentNaming

namespace LessBabeNoises.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NewBabePlusEnding.Actors.NBPKingEntity", "MakeBT")]
    public static class PatchNbpKingEntity
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        [UsedImplicitly]
        public static void Postfix(BehaviorTreeComp __result)
        {
            /* Sounds, in order played, are:
                1 - player.EndingParasol
                2 - babe.Jump
                3 - babe.Surprised
                4 - player.Jump
                5 - babe.Scream
             */

            if (!ModEntry.MuteNewBabe)
            {
                return;
            }

            var managerNodes = Traverse
                .Create(__result.GetRaw())
                .Field("m_root_node")
                .Field("m_children")
                .GetValue<IBTnode[]>();
            var btSequencor = managerNodes
                .First(node => node is BTsequencor);
            var traverseChildren = Traverse
                .Create(btSequencor)
                .Field("m_children");
            var filteredNodes = traverseChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = traverseChildren
                .SetValue(filteredNodes.ToArray());
        }
    }
}
