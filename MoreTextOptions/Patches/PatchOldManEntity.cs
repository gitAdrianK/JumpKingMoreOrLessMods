namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;

    [HarmonyPatch]
    public class PatchOldManEntity
    {
        public static MethodBase TargetMethod() =>
            AccessTools.Constructor(AccessTools.TypeByName("JumpKing.MiscEntities.OldManEntity"),
                new[] { typeof(OldManSettings) });

        // ReSharper disable once InconsistentNaming
        public static void Prefix(ref OldManSettings p_settings)
        {
            var translations = ModEntry.Translations;
            if (translations == null)
            {
                return;
            }

            TranslateIntroQuote(p_settings, translations);
            TranslateScreens(p_settings, translations);
            TranslateLoopingLines(p_settings, translations);
            TranslateFlagQuotes(p_settings, translations);
            TranslateFalls(p_settings, translations);
            TranslateItems(p_settings, translations);
        }

        private static void TranslateIntroQuote(
            OldManSettings settings,
            Dictionary<string, string> translations) =>
            TranslateLines(settings.intro_quote.lines, translations);

        private static void TranslateScreens(
            OldManSettings settings,
            Dictionary<string, string> translations)
        {
            if (settings.screens == null)
            {
                return;
            }

            foreach (var screen in settings.screens)
            {
                TranslateQuotes(screen.quotes, translations);
            }
        }

        private static void TranslateLoopingLines(
            OldManSettings settings,
            Dictionary<string, string> translations)
        {
            if (settings.looping_lines == null)
            {
                return;
            }

            foreach (var loopingLine in settings.looping_lines)
            {
                TranslateLines(loopingLine.lines, translations);
            }
        }

        private static void TranslateFlagQuotes(
            OldManSettings settings,
            Dictionary<string, string> translations)
        {
            if (settings.flag_quotes == null)
            {
                return;
            }

            foreach (var flagQuote in settings.flag_quotes)
            {
                TranslateQuotes(flagQuote.quotes, translations);
            }
        }

        private static void TranslateFalls(
            OldManSettings settings,
            Dictionary<string, string> translations)
        {
            if (settings.falls == null)
            {
                return;
            }

            foreach (var fall in settings.falls)
            {
                TranslateQuotes(fall.quotes, translations);
            }
        }

        private static void TranslateItems(
            OldManSettings settings,
            Dictionary<string, string> translations)
        {
            if (settings.items == null)
            {
                return;
            }

            foreach (var item in settings.items)
            {
                TranslateQuotes(item.quotes, translations);
            }
        }

        private static void TranslateQuotes(
            OldManQuote[] quotes,
            Dictionary<string, string> translations)
        {
            if (quotes == null)
            {
                return;
            }

            foreach (var quote in quotes)
            {
                TranslateLines(quote.lines, translations);
            }
        }

        private static void TranslateQuotes(
            List<OldManQuote> quotes,
            Dictionary<string, string> translations)
        {
            if (quotes == null)
            {
                return;
            }

            foreach (var quote in quotes)
            {
                TranslateLines(quote.lines, translations);
            }
        }

        private static void TranslateLines(
            string[] lines,
            IDictionary<string, string> translations)
        {
            if (lines == null)
            {
                return;
            }

            for (var i = 0; i < lines.Length; i++)
            {
                if (translations.TryGetValue(lines[i], out var translated))
                {
                    lines[i] = translated;
                }
            }
        }
    }
}
