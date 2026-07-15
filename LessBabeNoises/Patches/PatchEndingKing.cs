namespace LessBabeNoises.Patches
{
    using System.Linq;
    using BehaviorTree;
    using EntityComponent.BT;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.Util;

    [HarmonyPatch("JumpKing.GameManager.MultiEnding.NormalEnding.EndingKing", "MakeBT")]
    public static class PatchEndingKing
    {
        [UsedImplicitly]
        // ReSharper disable InconsistentNaming
        public static void Postfix(BehaviorTreeComp __result)
        {
            /* Sounds, in order played, are:
                1 - player.Jump
                2 - audio.Plink
                3 - player.Land
                4 - player.EndingParasol
                5 - babe.Surprised
                6 - babe.Surprised
                7 - player.Jump
                8 - babe.Scream

                1 - babe.Mou
             */
            if (!ModEntry.MuteMainBabe)
            {
                return;
            }

            var btSequencer = Traverse
                .Create(__result.GetRaw())
                .Field("m_root_node")
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var traverseChildren = Traverse
                .Create(btSequencer)
                .Field("m_children");
            var filteredNodes = traverseChildren
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            var ibtNodes = filteredNodes as IBTnode[] ?? filteredNodes.ToArray();
            _ = traverseChildren
                .SetValue(ibtNodes.ToArray());
            var btSimultaneous = ibtNodes
                .Last(node => node is BTsimultaneous);
            var btSequencer2 = Traverse
                .Create(btSimultaneous)
                .Field("m_children")
                .GetValue<IBTnode[]>()
                .First(node => node is BTsequencor);
            var traverseChildren2 = Traverse
                .Create(btSequencer2)
                .Field("m_children");
            var filteredNodes2 = traverseChildren2
                .GetValue<IBTnode[]>()
                .Where(node => !(node is PlaySFX));
            _ = traverseChildren2
                .SetValue(filteredNodes2.ToArray());
        }
    }
}
