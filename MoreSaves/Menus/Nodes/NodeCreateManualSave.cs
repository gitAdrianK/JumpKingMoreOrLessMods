namespace MoreSaves.Menus.Nodes
{
    using BehaviorTree;
    using JumpKing;

    /// <summary>
    ///     Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSave : IBTnode
    {
        protected override BTresult MyRun(TickData tickData)
        {
            CreateSave(ModEntry.SaveManager.SaveName);
            return BTresult.Success;
        }

        public static void CreateSave(string saveName)
        {
            try
            {
                ModEntry.SaveManager.SaveAllManual(saveName);
            }
            catch
            {
                ModEntry.SaveInfo?.SetText("> Failed to save.");
                Game1.instance.contentManager?.audio?.menu?.MenuFail?.Play();
            }

            ModEntry.SaveInfo?.SetText("> Saved successfully.");
            Game1.instance?.contentManager?.audio?.menu?.Select?.Play();
        }
    }
}
