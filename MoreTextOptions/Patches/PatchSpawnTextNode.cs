namespace MoreTextOptions.Patches
{
    using System.Collections.Generic;
    using System.Reflection;
    using HarmonyLib;
    using JumpKing.GameManager;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch]
    public static class PatchSpawnTextNode
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Constructor(typeof(SpawnTextNode),
                new[]
                {
                    typeof(string), typeof(Vector2), typeof(Vector2), typeof(FadeTextEntity.TimeSections),
                    typeof(Color),
                });

            yield return AccessTools.Constructor(typeof(SpawnTextNode),
                new[]
                {
                    typeof(string), typeof(Vector2), typeof(Vector2), typeof(FadeTextEntity.TimeSections),
                    typeof(Color), typeof(SpriteFont),
                });
        }

        // ReSharper disable once InconsistentNaming
        public static void Prefix(ref string p_text)
        {
            var translations = ModEntry.Translations;
            if (translations == null)
            {
                return;
            }

            if (translations.TryGetValue(
                    p_text, out var translated))
            {
                p_text = translated;
            }
        }
    }
}
