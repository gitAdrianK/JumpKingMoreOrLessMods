namespace MoreSaves.Saves
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JumpKing;
    using JumpKing.Level;
    using JumpKing.SaveThread;
    using JumpKing.SaveThread.SaveComponents;
    using JumpKing.Workshop;
    using LanguageJK;
    using Patches;
    using Util;
    using JKSaveManager = JumpKing.SaveThread.SaveManager;
    using Program = JumpKing.Program;

    // This class is a little overburdened, I'll clean it up later.

    /// <summary>
    ///     Manages the saving and loading of files that make up a JK save.
    /// </summary>
    public class SaveManager
    {
        /// <summary> The Saves folder, one of the folders files get written to. </summary>
        private const string Saves = "Saves";

        /// <summary> The SavesPerma folder, one of the folders files get written to. </summary>
        private const string SavesPerma = "SavesPerma";

        /// <summary> The name of the combined.sav file. </summary>
        private const string Combined = "combined.sav";

        /// <summary> The name of the attempt_stats.stat file. </summary>
        private const string AttemptStats = "attempt_stats.stat";

        /// <summary> The name of the event_flags.set file. </summary>
        private const string EventFlags = "event_flags.set";

        /// <summary> The name of the general_settings.set file. </summary>
        private const string GeneralSettings = "general_settings.set";

        /// <summary> The name of the inventory.inv file. </summary>
        private const string Inventory = "inventory.inv";

        /// <summary> The name of the perma_player_stats.stat file. </summary>
        private const string PermaPlayerStats = "perma_player_stats.stat";

        /// <summary> Regex for all filenames reserved by windows. </summary>
        private static readonly Regex ReservedNamesRegex =
            new Regex(@"^(con|prn|aux|nul|com\d|lpt\d)$", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Ctor.
        ///     Creates folders for the auto and manual saves inside the given dll directory.
        /// </summary>
        /// <param name="dllDirectory">Path of the dll containing directory</param>
        public SaveManager(string dllDirectory)
        {
            this.AutoDirectory = Path.Combine(dllDirectory, SaveType.Auto.ToLowerString());
            this.ManualDirectory = Path.Combine(dllDirectory, SaveType.Manual.ToLowerString());
            Directory.CreateDirectory(this.AutoDirectory);
            Directory.CreateDirectory(this.ManualDirectory);
        }

        /// <summary> Directory of auto saves. </summary>
        public string AutoDirectory { get; }

        /// <summary> Directory of manual saves. </summary>
        public string ManualDirectory { get; }

        /// <summary> Name the save will be saved under. </summary>
        public string SaveName { get; set; }

        /// <summary> Path to where the combined.sav file will be auto saved to. </summary>
        private string AutoCombinedFilePath { get; set; }
        /// <summary> Path to where the attempt_stats.stat file will be auto saved to. </summary>
        private string AutoAttemptStatsFilePath { get; set; }
        /// <summary> Path to where the event_flags.set file will be auto saved to. </summary>
        private string AutoEventFlagsFilePath { get; set; }
        /// <summary> Path to where the general_settings.set file will be auto saved to. </summary>
        private string AutoGeneralSettingsFilePath { get; set; }
        /// <summary> Path to where the inventory.inv file will be auto saved to. </summary>
        private string AutoInventoryFilePath { get; set; }
        /// <summary> Path to where the perma_player_stats.stat file will be auto saved to. </summary>
        private string AutoPermaPlayerStatsFilePath { get; set; }

        /// <summary> <c>true</c> if the name is not null or empty, <c>false</c> otherwise. </summary>
        private bool IsNameSet => !string.IsNullOrEmpty(this.SaveName);

        /// <summary>
        ///     Sets the save name that the save will be saved under. A name is required to be able to save.
        ///     As such setting the name effectively starts saving.
        /// </summary>
        public void StartSaving()
        {
            this.SaveName = this.SanitizeName(this.GetSaveName());

            // We additionally, to just setting the name, create required folders and paths,
            // so we don't have to do it every time we auto save.
            var saves = Path.Combine(this.AutoDirectory, this.SaveName, Saves);
            var savesPerma = Path.Combine(this.AutoDirectory, this.SaveName, SavesPerma);

            Directory.CreateDirectory(saves);
            Directory.CreateDirectory(savesPerma);

            this.AutoCombinedFilePath = Path.Combine(saves, Combined);
            this.AutoAttemptStatsFilePath = Path.Combine(savesPerma, AttemptStats);
            this.AutoEventFlagsFilePath = Path.Combine(savesPerma, EventFlags);
            this.AutoGeneralSettingsFilePath = Path.Combine(savesPerma, GeneralSettings);
            this.AutoInventoryFilePath = Path.Combine(savesPerma, Inventory);
            this.AutoPermaPlayerStatsFilePath = Path.Combine(savesPerma, PermaPlayerStats);
        }

        /// <summary>
        ///     Unsets the save name. A name is required to be able to save.
        ///     As such unsetting the name effectively stops saving.
        /// </summary>
        public void StopSaving() => this.SaveName = string.Empty;

        /// <summary>
        ///     Saves all files to the auto subdirectory with the current save name. The required folders are created
        ///     when the SaveManager starts saving.
        ///     Does not save if saving hasn't started.
        /// </summary>
        public void SaveAllAuto()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            PatchEncryption.SaveCombinedSaveFile(this.AutoCombinedFilePath, PatchSaveLube.CombinedSaveFile);
            PatchEncryption.SavePlayerStats(this.AutoAttemptStatsFilePath, PatchAchievementManager.Snapshot);
            PatchEncryption.SaveEventFlags(this.AutoEventFlagsFilePath, EventFlagsSave.Save);
            XmlSerializerHelper.Serialize(this.AutoGeneralSettingsFilePath, PatchSaveLube.GeneralSettings);
            PatchEncryption.SaveInventory(this.AutoInventoryFilePath, PatchSaveLube.Inventory);
            PatchEncryption.SavePlayerStats(this.AutoPermaPlayerStatsFilePath, PatchAchievementManager.AllTimeStats);
        }

        /// <summary>
        ///     Saves the combined.sav to the set auto directory with the set name.
        ///     Does not save if saving hasn't started.
        /// </summary>
        public void SaveCombined()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            PatchEncryption.SaveCombinedSaveFile(this.AutoCombinedFilePath, PatchSaveLube.CombinedSaveFile);
        }

        /// <summary>
        ///     Saves the general_settings.set to the set auto directory with the set name.
        ///     Does not save if saving hasn't started.
        /// </summary>
        public void SaveGeneralSettings()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            XmlSerializerHelper.Serialize(this.AutoGeneralSettingsFilePath, PatchSaveLube.GeneralSettings);
        }

        /// <summary>
        ///     Saves the inventory.inv to the set auto directory with the set name.
        ///     Does not save if saving hasn't started.
        /// </summary>
        public void SaveInventory()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            PatchEncryption.SaveInventory(this.AutoInventoryFilePath, PatchSaveLube.Inventory);
        }

        /// <summary>
        ///     Saves the perma_player_stats.stat to the set auto directory with the set name.
        ///     Does not save if saving hasn't started.
        /// </summary>
        public void SavePermaPlayerStats()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            PatchEncryption.SavePlayerStats(this.AutoPermaPlayerStatsFilePath, PatchAchievementManager.AllTimeStats);
        }

        public void SaveAllManual(string folderName)
        {
            var folderPath = Path.Combine(this.ManualDirectory, folderName);
            var savesPath =  Path.Combine(folderPath, Saves);
            var savesPermaPath = Path.Combine(folderPath, SavesPerma);

            Directory.CreateDirectory(savesPath);
            Directory.CreateDirectory(savesPermaPath);

            PatchEncryption.SaveCombinedSaveFile(Path.Combine(savesPath, Combined), PatchSaveLube.CombinedSaveFile);
            PatchEncryption.SavePlayerStats(Path.Combine(savesPermaPath, AttemptStats), PatchAchievementManager.Snapshot);
            PatchEncryption.SaveEventFlags(Path.Combine(savesPermaPath, EventFlags), EventFlagsSave.Save);
            XmlSerializerHelper.Serialize(Path.Combine(savesPermaPath, GeneralSettings), PatchSaveLube.GeneralSettings);
            PatchEncryption.SaveInventory(Path.Combine(savesPermaPath, Inventory), PatchSaveLube.Inventory);
            PatchEncryption.SavePlayerStats(Path.Combine(savesPermaPath, PermaPlayerStats), PatchAchievementManager.AllTimeStats);
        }

        /// <summary>
        ///     Loads all files and sets the required data for JK.
        /// </summary>
        /// <param name="directory">Directory containing save files.</param>
        /// <returns><c>true</c> if the load succeeded, <c>false</c> otherwise.</returns>
        public bool LoadSave(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            var contentManager = Game1.instance.contentManager;
            try
            {
                JKSaveManager.instance.StopSaving();
                Program.contentThread.Stop();

                var combined = PatchEncryption.LoadCombinedSaveFile(Path.Combine(directory, Saves, Combined));

                var attemptStats = PatchEncryption.LoadPlayerStats(Path.Combine(directory, SavesPerma, AttemptStats));
                var eventFlags = PatchEncryption.LoadEventFlags(Path.Combine(directory, SavesPerma, EventFlags));
                var generalSettings = XmlSerializerHelper.Deserialize<GeneralSettings>(
                    Path.Combine(directory, SavesPerma, GeneralSettings));
                var inventory = PatchEncryption.LoadInventory(Path.Combine(directory, SavesPerma, Inventory));
                var permaPlayerStats =
                    PatchEncryption.LoadPlayerStats(Path.Combine(directory, SavesPerma, PermaPlayerStats));

                // Root and level.
                string root;
                Level level = null;

                if (attemptStats.steam_level_id == null)
                {
                    root = "Content";
                }
                else
                {
                    level = WorkshopManager.instance.levels.First(lvl => lvl.ID == attemptStats.steam_level_id);
                    root = level.Root;
                }

                // Save and set
                contentManager.ReinitializeAssets();
                if (root == "Content")
                {
                    contentManager.SetLevel(root);
                }
                else
                {
                    contentManager.SetLevel(root, level);
                }

                PatchSaveLube.CombinedSaveFile = combined;

                PatchSaveLube.PlayerStatsAttemptSnapshot = attemptStats;
                PatchSaveLube.EventFlags = eventFlags;
                PatchSaveLube.GeneralSettings = generalSettings;
                PatchSaveLube.Inventory = inventory;
                PatchSaveLube.PermanentPlayerStats = permaPlayerStats;

                var options = new List<ItemEquipOptions.ItemOption>(generalSettings.item_options.Save.options);
                foreach (var option in options)
                {
                    PatchSkinManager.SetSkinEnabled(option.item, option.equipped);
                }

                PatchSaveLube.SaveCombinedSaveFile();
                PatchSaveLube.ProgramStartInitialize();

                PatchAchievementManager.Snapshot = attemptStats;
                PatchAchievementManager.AllTimeStats = permaPlayerStats;

                contentManager.LoadAssets(Game1.instance);
                LevelManager.LoadScreens();

                Game1.instance.m_game.UpdateMenu();
            }
            catch
            {
                return false;
            }
            finally
            {
                JKSaveManager.instance.StartSaving();
                PatchContentThread.Running = true;
            }

            return true;
        }

        /// <summary>
        ///     Deletes the auto save with the currently set name and stops saving.
        /// </summary>
        public void DeleteAutoSave()
        {
            if (!this.IsNameSet)
            {
                return;
            }

            var directory = Path.Combine(this.AutoDirectory, this.SaveName);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            this.StopSaving();
        }

        /// <summary>
        ///     Gets the name that is to be used for the folder the save files are to be saved into.
        /// </summary>
        /// <returns>Name of the save.</returns>
        private string GetSaveName()
        {
            var contentManager = Game1.instance.contentManager;

            // Not a vanilla map
            if (contentManager.level != null)
            {
                return contentManager.level.Name;
            }

            // Which vanilla map
            if (EventFlagsSave.ContainsFlag(StoryEventFlags.StartedNBP))
            {
                return language.GAMETITLESCREEN_NEW_BABE_PLUS;
            }

            return EventFlagsSave.ContainsFlag(StoryEventFlags.StartedGhost)
                ? language.GAMETITLESCREEN_GHOST_OF_THE_BABE
                : language.GAMETITLESCREEN_NEW_GAME;
        }

        /// <summary>
        ///     Sanitize names that contain invalid chars or are otherwise invalid names.
        ///     Invalid names will be suffixed with "#", invalid chars replaced with "#", and
        ///     the relative path syntax ".." split up into ". .".
        /// </summary>
        /// <param name="name">Name to sanitize.</param>
        /// <returns>Sanitized name.</returns>
        private string SanitizeName(string name)
        {
            name = name.Trim();

            if (string.IsNullOrEmpty(name))
            {
                return "emptyName#";
            }

            var invalidChars = Path.GetInvalidFileNameChars()
                .Concat(Path.GetInvalidPathChars())
                .Distinct();

            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '#');
            }

            // Windows special case: ".."
            name = name.Replace("..", ". .");

            // Windows reserved device names
            if (ReservedNamesRegex.IsMatch(name))
            {
                name = $"{name}#";
            }

            return name;
        }
    }
}
