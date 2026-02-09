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
    using SwitchBlocks.Patches;
    using Util;
    using JKSaveManager = JumpKing.SaveThread.SaveManager;

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

        /// <summary> <c>true</c> if the name is not null or empty, <c>false</c> otherwise. </summary>
        private bool IsNameSet => !string.IsNullOrEmpty(this.SaveName);

        /// <summary>
        ///     Sets the save name that the save will be saved under. A name is required to be able to save.
        ///     As such setting the name effectively starts saving.
        /// </summary>
        public void StartSaving() => this.SaveName = this.SanitizeName(this.GetSaveName());

        /// <summary>
        ///     Unsets the save name. A name is required to be able to save.
        ///     As such unsetting the name effectively stops saving.
        /// </summary>
        public void StopSaving() => this.SaveName = string.Empty;

        /// <summary>
        ///     Save all six files that make up a JK save file.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that the save files are contained in.</param>
        /// <returns></returns>
        private bool SaveAll(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            return this.SaveCombined(directory, folderName) &&
                   this.SaveAttemptStats(directory, folderName) &&
                   this.SaveEventFlags(directory, folderName) &&
                   this.SaveGeneralSettings(directory, folderName) &&
                   this.SaveInventory(directory, folderName) &&
                   this.SavePermaPlayerStats(directory, folderName);
        }

        /// <summary>
        ///     Saves all files to the auto subdirectory with the current save name.
        ///     As there won't be any feedback, it is not returned if the save succeeded.
        /// </summary>
        public void SaveAllAuto() => this.SaveAll(this.AutoDirectory, this.SaveName);

        /// <summary>
        ///     Saves all files to the manual subdirectory with the given save name.
        /// </summary>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveAllManual(string folderName) => this.SaveAll(this.ManualDirectory, folderName);

        /// <summary>
        ///     Saves the combined.sav to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveCombined(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, Saves);
            Directory.CreateDirectory(directory);
            PatchEncryption.SaveCombinedSaveFile(Path.Combine(directory, Combined), PatchSaveLube.CombinedSaveFile);
            return true;
        }

        /// <summary>
        ///     Saves the attempt_stats.stat to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveAttemptStats(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, SavesPerma);
            Directory.CreateDirectory(directory);
            PatchEncryption.SavePlayerStats(Path.Combine(directory, AttemptStats), PatchAchievementManager.Snapshot);
            return true;
        }

        /// <summary>
        ///     Saves the event_flags.set to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveEventFlags(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, SavesPerma);
            Directory.CreateDirectory(directory);
            PatchEncryption.SaveEventFlags(Path.Combine(directory, EventFlags), EventFlagsSave.Save);
            return true;
        }

        /// <summary>
        ///     Saves the general_settings.set to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveGeneralSettings(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, SavesPerma);
            Directory.CreateDirectory(directory);
            XmlSerializerHelper.Serialize(Path.Combine(directory, GeneralSettings), PatchSaveLube.GeneralSettings);
            return true;
        }

        /// <summary>
        ///     Saves the inventory.inv to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SaveInventory(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, SavesPerma);
            Directory.CreateDirectory(directory);
            PatchEncryption.SaveInventory(Path.Combine(directory, Inventory), PatchSaveLube.Inventory);
            return true;
        }

        /// <summary>
        ///     Saves the perma_player_stats.stat to the given directory with the given save name.
        /// </summary>
        /// <param name="directory">Directory to save to.</param>
        /// <param name="folderName">Name of the folder that will contain the save files.</param>
        /// <returns><c>true</c> if the save succeeded, <c>false</c> otherwise.</returns>
        public bool SavePermaPlayerStats(string directory, string folderName)
        {
            if (!this.IsNameSet)
            {
                return false;
            }

            directory = Path.Combine(directory, folderName, SavesPerma);
            Directory.CreateDirectory(directory);
            PatchEncryption.SavePlayerStats(Path.Combine(directory, PermaPlayerStats),
                PatchAchievementManager.AllTimeStats);
            return true;
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
