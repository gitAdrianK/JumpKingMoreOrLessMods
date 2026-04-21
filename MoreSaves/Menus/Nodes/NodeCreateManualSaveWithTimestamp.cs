namespace MoreSaves.Menus.Nodes
{
    using System;
    using BehaviorTree;

    /// <summary>
    ///     Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSaveWithTimestamp : IBTnode
    {
        protected override BTresult MyRun(TickData tickData)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var folderName = $"{ModEntry.SaveManager.SaveName}-{date}";
            NodeCreateManualSave.CreateSave(folderName);
            return BTresult.Success;
        }
    }
}
