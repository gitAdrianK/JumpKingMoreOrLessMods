namespace MoreSaves.Menus.Nodes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using BehaviorTree;
    using JetBrains.Annotations;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using JumpKing.SaveThread;
    using Patches;

    /// <summary>
    ///     Node that creates a manual save with the name of the map and a slightly modified ISO 8601.
    /// </summary>
    public class NodeCreateManualSave : IBTnode
    {
        private const string Manual = ModStrings.Manual;
        private const string SavesPerma = ModStrings.SavesPerma;

        [PauseMenuItemSetting]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for JK")]
        [UsedImplicitly]
        public static TextButton CreateManualSave(object factory, GuiFormat format)
            => new TextButton("Create Manual Save", new NodeCreateManualSave());

        protected override BTresult MyRun(TickData pData)
        {
            if (ModEntry.SaveName == string.Empty)
            {
                Game1.instance.contentManager.audio.menu.MenuFail.Play();
                return BTresult.Failure;
            }

            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var directoryName = $"{ModEntry.SaveName}-{date}";

            PatchXmlWrapper.Serialize(PatchSaveLube.GetGeneralSettings(), Manual, directoryName, SavesPerma);
            PatchEncryption.SaveInventory(PatchInventoryManager.GetInventory(), Manual, directoryName, SavesPerma);
            PatchEncryption.SaveEventFlags(EventFlagsSave.Save, Manual, directoryName, SavesPerma);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPlayerStats(), ModStrings.Stats, Manual,
                directoryName, SavesPerma);
            PatchEncryption.SavePlayerStats(PatchAchievementManager.GetPermaStats(), ModStrings.Permanent, Manual,
                directoryName, SavesPerma);
            PatchEncryption.SaveCombinedSaveFile(PatchSaveLube.GetCombinedSaveFile(), Manual, directoryName,
                ModStrings.Saves);

            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
