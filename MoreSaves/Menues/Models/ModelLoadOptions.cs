namespace MoreSaves.Menues.Models
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.Mods;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using LanguageJK;
    using Microsoft.Xna.Framework;
    using MoreSaves.Menues.Nodes;

    public class ModelLoadOptions
    {
        [MainMenuItemSetting]
        public static TextButton LoadAutoSavefile(object factory, GuiFormat format)
        {
            SetupButtons();
            return new TextButton("Load Automatic Save",
                CreateLoadOptions(factory, format, 0, PageOption.Auto));
        }

        [MainMenuItemSetting]
        public static TextButton LoadManualSavefile(object factory, GuiFormat format)
        {
            SetupButtons();
            return new TextButton("Load Manual Save",
                CreateLoadOptions(factory, format, 0, PageOption.Manual));
        }

        public enum PageOption
        {
            Auto,
            Manual,
        }

        private const string AUTO = ModStrings.AUTO;
        private const string MANUAL = ModStrings.MANUAL;

        /// <summary>
        /// Maximum amount of buttons per page.
        /// </summary>
        private const int AMOUNT = 9;

        /// <summary>
        /// The buttons that the auto page can hold.
        /// </summary>
        private static List<TextButton> autoButtons;

        /// <summary>
        /// The buttons that the manual page can hold.
        /// </summary>
        private static List<TextButton> manualButtons;

        /// <summary>
        /// Reads the auto and manual directories and creates a button for each folder found inside.
        /// </summary>
        public static void SetupButtons()
        {
            var sep = Path.DirectorySeparatorChar;
            var dllDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{sep}";
            var autoDirectories = Directory.GetDirectories($"{dllDirectory}{AUTO}{sep}");
            var manualDirectories = Directory.GetDirectories($"{dllDirectory}{MANUAL}{sep}");
            var menuFontSmall = Game1.instance.contentManager.font.MenuFontSmall;

            autoButtons = new List<TextButton>();
            foreach (var directory in autoDirectories)
            {
                var dir = directory.Split(sep).Last();
                autoButtons.Add(new TextButton(CropName(dir), new NodeLoadSave(AUTO, dir), menuFontSmall));
            }
            manualButtons = new List<TextButton>();
            foreach (var directory in manualDirectories)
            {
                var dir = directory.Split(sep).Last();
                manualButtons.Add(new TextButton(CropName(dir), new NodeLoadSave(MANUAL, dir), menuFontSmall));
            }
        }

        /// <summary>
        /// Crops the name should it be longer than 60 characters as it would cause an overflow.
        /// The name will be cropped at 57 characters and "..." will be inserted at the front to indicate the name having been cropped.
        /// </summary>
        /// <param name="name">The name to be cropped</param>
        /// <returns>The cropped name</returns>
        private static string CropName(string name)
        {
            if (name.Length > 60)
            {
                name = $"...{name.Substring(name.Length - 57)}";
            }
            return name;
        }

        public static MenuSelectorClosePopup CreateLoadOptions(object factory, GuiFormat format, int page, PageOption pageOption)
        {
            List<TextButton> buttons;
            switch (pageOption)
            {
                case PageOption.Auto:
                    buttons = autoButtons;
                    break;
                case PageOption.Manual:
                    buttons = manualButtons;
                    break;
                default:
                    buttons = new List<TextButton>();
                    break;
            }

            if (buttons.Count() == 0)
            {
                var emptySelector = new MenuSelectorClosePopup(format);
                emptySelector.AddChild(new TextInfo("No saves to load.", Color.Gray));
                emptySelector.Initialize();
                return emptySelector;
            }

            var menuFactory = Traverse.Create(factory);
            var guiFormat = menuFactory.Field("GUI_FORMAT").GetValue<GuiFormat>();
            var customFormat = guiFormat;
            customFormat.anchor = Vector2.Zero;

            var menuSelector = new MenuSelectorClosePopup(customFormat);

            menuSelector.AddChild(new TextInfo("Load Save!", Color.White));

            var num = 0;
            for (var i = page * AMOUNT; i < (page * AMOUNT) + AMOUNT; i++)
            {
                if (num == AMOUNT)
                {
                    break;
                }

                if (i < buttons.Count)
                {
                    menuSelector.AddChild(buttons[i]);
                }

                num++;
            }

            if (page > 0)
            {
                menuSelector.AddChild(new TextButton(language.PAGINATION_PREVIOUS, new MenuSelectorBack(menuSelector)));
            }
            if ((page * AMOUNT) + num < buttons.Count)
            {
                menuSelector.AddChild(new TextButton(language.PAGINATION_NEXT, CreateLoadOptions(factory, format, page + 1, pageOption)));
            }

            menuSelector.Initialize();
            return menuSelector;
        }
    }
}
