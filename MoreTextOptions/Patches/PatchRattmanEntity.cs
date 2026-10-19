namespace MoreTextOptions.Patches
{
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.Props.RattmanText;

    [HarmonyPatch]
    public static class PatchRattmanEntity
    {
        public static MethodBase TargetMethod() =>
            AccessTools.Constructor(AccessTools.TypeByName("JumpKing.Props.RattmanText.RattmanEntity"),
                new[] { typeof(RattmanSettings) });

        // ReSharper disable once InconsistentNaming
        public static void Prefix(ref RattmanSettings p_settings)
        {
            var lines = p_settings.text.lines;
            var translations = ModEntry.Translations;
            if (translations == null || lines == null)
            {
                return;
            }

            for (var i = 0; i < lines.Length; i++)
            {
                if (translations.TryGetValue(
                        lines[i], out var translated))
                {
                    lines[i] = translated;
                }
            }
        }
    }
}
