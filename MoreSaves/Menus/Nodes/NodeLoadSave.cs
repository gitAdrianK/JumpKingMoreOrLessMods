namespace MoreSaves.Menus.Nodes
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BehaviorTree;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Level;
    using JumpKing.MiscEntities.WorldItems.Inventory;
    using JumpKing.MiscSystems.Achievements;
    using JumpKing.SaveThread;
    using JumpKing.SaveThread.SaveComponents;
    using JumpKing.Workshop;
    using Patches;

    /// <summary>
    ///     Node to load a save from the mod into Jump King.
    ///     All required fields will be set and the JK menu will reload/update.
    /// </summary>
    public class NodeLoadSave : IBTnode
    {
        private const string Saves = ModStrings.Saves;
        private const string SavesPerma = ModStrings.SavesPerma;
        private const string Combined = ModStrings.Combined;
        private const string Event = ModStrings.Event;
        private const string Stats = ModStrings.Stats;
        private const string Permanent = ModStrings.Permanent;
        private const string Inventory = ModStrings.Inventory;
        private const string Settings = ModStrings.Settings;

        private const string Content = ModStrings.Content;
        private static readonly char Sep;

        private static readonly JKContentManager ContentManager;
        private static readonly SaveManager SaveManager;

        private static readonly MethodInfo SetCombinedSave;
        private static readonly MethodInfo SetPlayerStats;
        private static readonly MethodInfo SetPermanentStats;
        private static readonly MethodInfo SetInventory;
        private static readonly MethodInfo SetGeneralSettings;

        private static readonly MethodInfo SetSkinEnabled;

        private static readonly MethodInfo LoadCombinedSaveFile;
        private static readonly MethodInfo LoadEventFlags;
        private static readonly MethodInfo LoadPlayerStats;
        private static readonly MethodInfo LoadInventory;

        private static readonly MethodInfo SaveProgramStartInitialize;
        private static readonly MethodInfo SaveCombinedSaveFile;

        private static readonly Traverse TraversePlayerStats;
        private static readonly Traverse TraversePermaStats;

        private readonly string directory;

        static NodeLoadSave()
        {
            Sep = Path.DirectorySeparatorChar;

            // Classes and methods.
            ContentManager = Game1.instance.contentManager;
            SaveManager = SaveManager.instance;

            var saveLube = AccessTools.TypeByName("JumpKing.SaveThread.SaveLube");
            var encryption = AccessTools.TypeByName("FileUtil.Encryption.Encryption");
            var achievementManager = AccessTools.TypeByName("JumpKing.MiscSystems.Achievements.AchievementManager");
            var skinManager = AccessTools.TypeByName("JumpKing.Player.Skins.SkinManager");

            SetCombinedSave = saveLube.GetMethod("set_CombinedSave");
            SetPlayerStats = saveLube.GetMethod("set_PlayerStatsAttemptSnapshot");
            SetPermanentStats = saveLube.GetMethod("set_PermanentPlayerStats");
            SetInventory = saveLube.GetMethod("set_inventory");
            SetGeneralSettings = saveLube.GetMethod("set_generalSettings");

            SetSkinEnabled = skinManager.GetMethod("SetSkinEnabled");

            var loadFile = encryption.GetMethod("LoadFile");
            LoadCombinedSaveFile = loadFile.MakeGenericMethod(typeof(CombinedSaveFile));
            LoadEventFlags = loadFile.MakeGenericMethod(typeof(EventFlagsSave));
            LoadPlayerStats = loadFile.MakeGenericMethod(typeof(PlayerStats));
            LoadInventory = loadFile.MakeGenericMethod(typeof(Inventory));

            SaveProgramStartInitialize = saveLube.GetMethod("ProgramStartInitialize");
            SaveCombinedSaveFile = saveLube.GetMethod("SaveCombinedSaveFile");

            var achievementManagerInstance = achievementManager
                .GetField("instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var achievementManagerTraverse = Traverse.Create(achievementManagerInstance);
            TraversePlayerStats = achievementManagerTraverse.Field("m_snapshot");
            TraversePermaStats = achievementManagerTraverse.Field("m_all_time_stats");
        }

        public NodeLoadSave(params string[] folders)
        {
            this.directory = ModEntry.DllDirectory;
            foreach (var folder in folders)
            {
                this.directory += folder + Sep;
            }
        }

        protected override BTresult MyRun(TickData pData)
        {
            try
            {
                SaveManager.StopSaving();

                // Load from dllDirectory
                var combinedSaveFile = (CombinedSaveFile)LoadCombinedSaveFile.Invoke(null,
                    new object[] { $"{this.directory}{Sep}{Saves}{Sep}{Combined}" });
                var eventFlags = (EventFlagsSave)LoadEventFlags.Invoke(null,
                    new object[] { $"{this.directory}{Sep}{SavesPerma}{Sep}{Event}" });
                var playerStats = (PlayerStats)LoadPlayerStats.Invoke(null,
                    new object[] { $"{this.directory}{Sep}{SavesPerma}{Sep}{Stats}" });
                var permaStats = (PlayerStats)LoadPlayerStats.Invoke(null,
                    new object[] { $"{this.directory}{Sep}{SavesPerma}{Sep}{Permanent}" });
                var inventory = (Inventory)LoadInventory.Invoke(null,
                    new object[] { $"{this.directory}{Sep}{SavesPerma}{Sep}{Inventory}" });
                var generalSettings =
                    XmlSerializerHelper.Deserialize<GeneralSettings>(
                        $"{this.directory}{Sep}{SavesPerma}{Sep}{Settings}");

                // Root and level.
                string root;
                Level level = null;

                if (playerStats.steam_level_id == null)
                {
                    root = Content;
                }
                else
                {
                    level = WorkshopManager.instance.levels.First(lvl => lvl.ID == playerStats.steam_level_id);
                    root = level.Root;
                }

                // Save and set
                ContentManager.ReinitializeAssets();

                if (root == Content)
                {
                    ContentManager.SetLevel(root);
                }
                else
                {
                    ContentManager.SetLevel(root, level);
                }

                _ = SetCombinedSave.Invoke(null, new object[] { combinedSaveFile });
                _ = SetPlayerStats.Invoke(null, new object[] { playerStats });
                _ = SetPermanentStats.Invoke(null, new object[] { permaStats });
                _ = SetInventory.Invoke(null, new object[] { inventory });
                PatchInventoryManager.SetInventory(inventory);
                _ = SetGeneralSettings.Invoke(null, new object[] { generalSettings });
                EventFlagsSave.Save = eventFlags;

                var options = new List<ItemEquipOptions.ItemOption>(generalSettings.item_options.Save.options);
                foreach (var option in options)
                {
                    _ = SetSkinEnabled.Invoke(null, new object[] { option.item, option.equipped });
                }

                _ = SaveCombinedSaveFile.Invoke(null, null);
                _ = SaveProgramStartInitialize.Invoke(null, null);

                _ = TraversePlayerStats.SetValue(playerStats);
                _ = TraversePermaStats.SetValue(permaStats);

                ContentManager.LoadAssets(Game1.instance);
                LevelManager.LoadScreens();

                ContentManager.audio.menu.Select.Play();
                Game1.instance.m_game.UpdateMenu();
            }
            catch
            {
                ContentManager.audio.menu.MenuFail.Play();
                return BTresult.Failure;
            }
            finally
            {
                SaveManager.StartSaving();
            }

            return BTresult.Success;
        }
    }
}
