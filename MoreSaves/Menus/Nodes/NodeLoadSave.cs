namespace MoreSaves.Menus.Nodes
{
    using BehaviorTree;
    using JumpKing;

    /// <summary>
    ///     Node to load a save from the mod into Jump King.
    ///     All required fields will be set and the JK menu will reload/update.
    /// </summary>
    public class NodeLoadSave : IBTnode
    {
        public NodeLoadSave(string directory) => this.Directory = directory;
        private string Directory { get; }

        protected override BTresult MyRun(TickData pData)
        {
            if (!ModEntry.SaveManager.LoadSave(this.Directory))
            {
                Game1.instance.contentManager.audio.menu.MenuFail.Play();
                return BTresult.Failure;
            }

            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
