namespace MoreSaves.Menus.Nodes
{
    using System;
    using BehaviorTree;
    using JumpKing;

    /// <summary>
    ///     Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSave : IBTnode
    {
        protected override BTresult MyRun(TickData pData)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var folderName = $"{ModEntry.SaveManager.SaveName}-{date}";

            ModEntry.SaveManager.SaveAllManual(folderName);

            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
