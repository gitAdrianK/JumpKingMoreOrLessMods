// ReSharper disable InconsistentNaming

namespace MoreTextOptions.Patches
{
    using System;
    using HarmonyLib;
    using JetBrains.Annotations;
    using JumpKing.MiscEntities.OldMan;
    using Microsoft.Xna.Framework.Graphics;

    [HarmonyPatch("JumpKing.MiscEntities.OldManEntity", "GetOldManFont")]
    public static class PatchOldManEntity
    {
        public static SpriteFont Default { get; set; }
        public static SpriteFont Gargoyle { get; set; }

        [UsedImplicitly]
        public static bool Prefix(OldManFont p_font, ref SpriteFont __result)
        {
            switch (p_font)
            {
                case OldManFont.Default:
                    if (Default != null)
                    {
                        __result = Default;
                        return false;
                    }

                    break;
                case OldManFont.Gargoyle:
                    if (Gargoyle != null)
                    {
                        __result = Gargoyle;
                        return false;
                    }

                    break;
                default:
                    throw new Exception("Invalid Font Enum");
            }

            return true;
        }
    }
}
