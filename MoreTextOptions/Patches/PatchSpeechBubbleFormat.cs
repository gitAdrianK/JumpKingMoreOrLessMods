// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpeechBubbleFormat), nameof(SpeechBubbleFormat.ChopString), typeof(string), typeof(SpriteFont),
        typeof(int), typeof(char[]))]
    public static class PatchSpeechBubbleFormat
    {
        public static void Prefix(ref string full_str, out PatchState __state)
        {
            __state = null;
            var matches = ModEntry.Regex.Matches(full_str);
            if (matches.Count == 0)
            {
                return;
            }

            var lengths = new List<int>();
            foreach (Match match in matches)
            {
                lengths.Add(match.Length);
            }

            __state = new PatchState { FullString = full_str, Lengths = lengths };

            full_str = ModEntry.Regex.Replace(full_str, string.Empty);
        }


        public static void Postfix(ref List<string> __result, PatchState __state)
        {
            if (__state == null || __result == null)
            {
                return;
            }

            var fullStr = __state.FullString;
            var lengths = __state.Lengths;
            var result = new List<string>(__result.Count);

            var activeColor = string.Empty;
            var activeFont = string.Empty;
            var strIndex = 0;
            var lengthIndex = 0;

            foreach (var chop in __result)
            {
                var sb = new StringBuilder(chop.Length);
                var isStart = true;

                foreach (var c in chop)
                {
                    if (c != fullStr[strIndex])
                    {
                        var length = lengths[lengthIndex++];
                        var tag = fullStr.Substring(strIndex, length);

                        var color = ModEntry.RegexColor.Match(tag);
                        if (color.Success)
                        {
                            activeColor = color.Value;
                        }

                        var font = ModEntry.RegexFont.Match(tag);
                        if (font.Success)
                        {
                            activeFont = font.Value;
                        }

                        if (isStart)
                        {
                            var fullTag = GetFullTag(activeColor, activeFont);
                            if (fullTag.Length > 0)
                            {
                                sb.Append('{').Append(fullTag).Append('}');
                            }
                        }
                        else
                        {
                            sb.Append(tag);
                        }

                        strIndex += length;
                        isStart = false;
                    }
                    else if (isStart && !char.IsWhiteSpace(c))
                    {
                        var fullTag = GetFullTag(activeColor, activeFont);
                        if (fullTag.Length > 0)
                        {
                            sb.Append('{').Append(fullTag).Append('}');
                        }

                        isStart = false;
                    }

                    sb.Append(c);
                    strIndex++;
                }

                result.Add(sb.ToString());
            }

            __result = result;
        }

        private static string GetFullTag(string activeColor, string activeFont) =>
            string.IsNullOrEmpty(activeColor) ? activeFont :
            string.IsNullOrEmpty(activeFont) ? activeColor :
            $"{activeColor} {activeFont}";

        public class PatchState
        {
            public string FullString { get; set; }
            public List<int> Lengths { get; set; }
        }
    }
}
