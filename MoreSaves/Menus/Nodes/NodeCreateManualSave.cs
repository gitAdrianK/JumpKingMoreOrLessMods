namespace MoreSaves.Menus.Nodes
{
    using BehaviorTree;
    using JumpKing;

    /// <summary>
    ///     Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSave : IBTnode
    {
        protected override BTresult MyRun(TickData pData)
        {
            ModEntry.SaveManager.SaveAllManual(ModEntry.SaveManager.SaveName);

            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
