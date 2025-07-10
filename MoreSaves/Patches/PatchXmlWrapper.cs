namespace MoreSaves.Patches
{
    using System.IO;
    using JumpKing.SaveThread;

    public static class PatchXmlWrapper
    {
        private static readonly char Sep;

        static PatchXmlWrapper() => Sep = Path.DirectorySeparatorChar;

        public static void Serialize(GeneralSettings generalSettings, params string[] folders)
        {
            var path = BuildAndCreatePath(folders) + Sep + ModStrings.Settings;
            XmlSerializerHelper.Serialize(path, generalSettings);
        }

        /// <summary>
        ///     Builds a path from given folders, starting from the path to the dll. If the path doesn't exists creates it.
        /// </summary>
        /// <param name="folders">The folders making up the path to the save, starting from the path to the dll</param>
        /// <returns>The path</returns>
        private static string BuildAndCreatePath(params string[] folders)
        {
            var path = ModEntry.DllDirectory;
            foreach (var folder in folders)
            {
                path += folder + Sep;
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }
            }

            return path;
        }
    }
}
