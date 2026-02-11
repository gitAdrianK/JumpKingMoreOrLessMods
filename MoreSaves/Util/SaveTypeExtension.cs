namespace MoreSaves.Util
{
    public static class SaveTypeExtension
    {
        public static string ToLowerString(this SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Auto:
                    return "auto";
                case SaveType.Manual:
                    return "manual";
                default:
                    return saveType.ToString().ToLowerInvariant();
            }
        }
    }
}
