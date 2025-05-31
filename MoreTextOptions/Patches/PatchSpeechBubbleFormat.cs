namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using HarmonyLib;
    using JumpKing.MiscEntities.OldMan;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch(typeof(SpeechBubbleFormat), nameof(SpeechBubbleFormat.ChopString),
        new[] { typeof(string), typeof(SpriteFont), typeof(int), typeof(char[]) })]
    public static class PatchSpeechBubbleFormat
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        public static bool Prefix(out string __state, ref string full_str)
        {
            __state = string.Empty;
            if (ModEntry.REGEX.IsMatch(full_str))
            {
                __state = full_str;
                full_str = ModEntry.REGEX.Replace(full_str, string.Empty);
            }
            return true;
        }

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony naming convention")]
        public static void Postfix(string __state, ref List<string> __result)
        {
            if (__state == string.Empty || __result == null)
            {
                return;
            }

            var resultWithTags = new List<string>();
            var color = string.Empty;
            var index = 0;

            foreach (var chop in __result)
            {
                var stringBuilder = new StringBuilder();
                var isStart = true;

                foreach (var c in chop)
                {
                    if (c != __state[index])
                    {
                        color = __state.Substring(index, 17);
                        _ = stringBuilder.Append(color);
                        index += 17;
                        isStart = false;
                    }
                    else
                    {
                        if (isStart && !string.IsNullOrWhiteSpace(c.ToString()))
                        {
                            _ = stringBuilder.Append(color);
                            isStart = false;
                        }
                    }

                    _ = stringBuilder.Append(c);
                    index++;
                }

                resultWithTags.Add(stringBuilder.ToString());
            }

            __result = resultWithTags;
        }
    }
}
