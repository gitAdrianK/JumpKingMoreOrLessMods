namespace LessBabeNoises.Patches
{
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.OwlEnding.OwlKingEntity", "MakeBT")]
    public static class PatchOwlKingEntity
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
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

            var btSequencer = Traverse
                .Create(__result.GetRaw())
                .Field("m_root_node")
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var sequencerChildren = Traverse
                .Create(btSequencer)
                .Field("m_children");
            var filteredNodes = sequencerChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            var ibtNodes = filteredNodes as IBTnode[] ?? filteredNodes.ToArray();
            _ = sequencerChildren
                .SetValue(ibtNodes.ToArray());
            var btSimultaneous = ibtNodes
                .Last(node => node is BTsimultaneous);
            var btSequencer2 = Traverse
                .Create(btSimultaneous)
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var traverseSequencer2 = Traverse
                .Create(btSequencer2)
                .Field("m_children");
            var filteredNodes2 = traverseSequencer2
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = traverseSequencer2
                .SetValue(filteredNodes2.ToArray());
        }
    }
}
