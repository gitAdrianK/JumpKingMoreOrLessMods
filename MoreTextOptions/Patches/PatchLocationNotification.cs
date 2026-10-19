namespace MoreTextOptions.Patches
{
    using System.Reflection;
    using HarmonyLib;

    [HarmonyPatch]
    public static class PatchLocationNotification
    {
        public static MethodBase TargetMethod() =>
            AccessTools.Constructor(AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationNotification"),
                new[] { typeof(string) });

        // ReSharper disable once InconsistentNaming
        public static void Prefix(ref string p_name)
        {
            var translations = ModEntry.Translations;
            if (translations == null)
            {
                return;
            }

            if (translations.TryGetValue(
                    p_name, out var translated))
            {
                p_name = translated;
            }
        }
    }
}
