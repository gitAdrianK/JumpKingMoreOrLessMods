namespace MoreSaves.Menus.Models
{
    using System.Collections.Generic;
    using System.IO;
    using HarmonyLib;
    using JumpKing;
    using JumpKing.PauseMenu;
    using JumpKing.PauseMenu.BT;
    using LanguageJK;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Nodes;
    using Util;

    public static class ModelLoadOptions
    {
        /// <summary>
        ///     Maximum amount of buttons per page.
        /// </summary>
        private const int Amount = 9;

        /// <summary>
        ///     The buttons that the auto page can hold.
        /// </summary>
        private static List<TextButton> autoButtons;

        /// <summary>
        ///     The buttons that the manual page can hold.
        /// </summary>
        private static List<TextButton> manualButtons;

        /// <summary>
        ///     Reads the auto and manual directories and creates a button for each folder found inside.
        /// </summary>
        public static void SetupButtons()
        {
            var menuFontSmall = Game1.instance.contentManager.font.MenuFontSmall;

            autoButtons = CreateButtons(SaveType.Auto.ToLowerString(), menuFontSmall);
            manualButtons = CreateButtons(SaveType.Manual.ToLowerString(), menuFontSmall);
        }

        private static List<TextButton> CreateButtons(string saveType, SpriteFont menuFont)
        {
            var buttons = new List<TextButton>();

            var pathToSaveType = Path.Combine(ModEntry.DllDirectory, saveType);
            foreach (var directory in Directory.GetDirectories(pathToSaveType))
            {
                var info = new DirectoryInfo(directory);
                buttons.Add(new TextButton(CropName(info.Name), new NodeLoadSave(info.FullName), menuFont));
            }

            return buttons;
        }

        /// <summary>
        ///     Crops the name should it be longer than 60 characters as it would cause a visual overflow.
        ///     The name will be cropped at 57 characters and "..." will be inserted at the front to indicate
        ///     the name having been cropped.
        /// </summary>
        /// <param name="name">Name to be cropped.</param>
        /// <returns>Cropped name.</returns>
        private static string CropName(string name)
        {
            if (name.Length > 60)
            {
                name = $"...{name.Substring(name.Length - 57)}";
            }

            return name;
        }

        public static MenuSelectorClosePopup CreateLoadOptions(object factory, GuiFormat format, int page,
            SaveType saveType)
        {
            List<TextButton> buttons;
            switch (saveType)
            {
                case SaveType.Auto:
                    buttons = autoButtons;
                    break;
                case SaveType.Manual:
                    buttons = manualButtons;
                    break;
                default:
                    buttons = new List<TextButton>();
                    break;
            }

            if (buttons.Count == 0)
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
            for (var i = page * Amount; i < (page * Amount) + Amount; i++)
            {
                if (num == Amount)
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

            if ((page * Amount) + num < buttons.Count)
            {
                menuSelector.AddChild(new TextButton(language.PAGINATION_NEXT,
                    CreateLoadOptions(factory, format, page + 1, saveType)));
            }

            menuSelector.Initialize();
            return menuSelector;
        }
    }
}
