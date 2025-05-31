namespace LessBabeNoises.Patches
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.OwlEnding.OwlKingEntity", "MakeBT")]
    public class PatchOwlKingEntity
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        public static void RemoveBabeNoises(BehaviorTreeComp __result)
        {
            /* Sounds, in order played, are:
                1 - audio.Plink
                2 - babe.Pickup
                3 - babe.Surprised
                
                (Unused)
                1 - player.Jump

                1 - player.Splat
                2 - babe.Scream
                3 - babe.Kiss
             */

            if (!ModEntry.MuteGhostBabe)
            {
                return;
            }
            var btSequencor = Traverse
                .Create(__result.GetRaw())
                .Field("m_root_node")
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var sequencorChildren = Traverse
                .Create(btSequencor)
                .Field("m_children");
            var filteredNodes = sequencorChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = sequencorChildren
                .SetValue(filteredNodes.ToArray());
            var btSimultaneos = filteredNodes
                .Last(node => node is BTsimultaneous);
            var btSequencor2 = Traverse
                .Create(btSimultaneos)
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var traverseSequencor2 = Traverse
                .Create(btSequencor2)
                .Field("m_children");
            var filteredNodes2 = traverseSequencor2
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = traverseSequencor2
                .SetValue(filteredNodes2.ToArray());
        }
    }
}
