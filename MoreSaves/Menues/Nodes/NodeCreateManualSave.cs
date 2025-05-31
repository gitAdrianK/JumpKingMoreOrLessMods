namespace MoreSaves.Menues.Nodes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using BehaviorTree;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using JumpKing.SaveThread;
    using MoreSaves.Patches;

    /// <summary>
    /// Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSave : IBTnode
    {
        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        public static TextButton CreateManualSave(object factory, GuiFormat format)
            => new TextButton("Create Manual Save", new NodeCreateManualSave());


        private const string MANUAL = ModStrings.MANUAL;
        private const string SAVES_PERMA = ModStrings.SAVES_PERMA;

        protected override BTresult MyRun(TickData p_data)
        {
            if (ModEntry.SaveName == string.Empty)
            {
                Game1.instance.contentManager.audio.menu.MenuFail.Play();
                return BTresult.Failure;
            }

            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var directoryName = $"{ModEntry.SaveName}-{date}";

            PatchXmlWrapper.Serialize(PatchSaveLube.GetGeneralSettings(), MANUAL, directoryName, SAVES_PERMA);
            PatchEncryption.SaveInventory(PatchInventoryManager.GetInventory(), MANUAL, directoryName, SAVES_PERMA);
            PatchEncryption.SaveEventFlags(EventFlagsSave.Save, MANUAL, directoryName, SAVES_PERMA);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPlayerStats(), ModStrings.STATS, MANUAL, directoryName, SAVES_PERMA);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPermaStats(), ModStrings.PERMANENT, MANUAL, directoryName, SAVES_PERMA);
            PatchEncryption.SaveCombinedSaveFile(PatchSaveLube.GetCombinedSaveFile(), MANUAL, directoryName, ModStrings.SAVES);

            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
