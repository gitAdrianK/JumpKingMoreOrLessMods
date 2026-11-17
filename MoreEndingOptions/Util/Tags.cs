namespace MoreEndingOptions.Util
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Entities;
    using HarmonyLib;
    using JumpKing.GameManager.MultiEnding;
    using JumpKing.Util.DrawBT;
    using Microsoft.Xna.Framework;

    public static class Tags
    {
        public static bool HandleSimpleTags(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            switch (tag)
            {
                case "MuteMainBabe":
                    ModEntry.MuteMainBabe = true;
                    BtWalker.RemoveBabeNoises(mainEnding);
                    return true;
                case "MuteNewBabe":
                    ModEntry.MuteNewBabe = true;
                    BtWalker.RemoveBabeNoises(newEnding);
                    return true;
                case "MuteGhostBabe":
                    ModEntry.MuteGhostBabe = true;
                    BtWalker.RemoveBabeNoises(ghostEnding);
                    return true;
                case "ShortMainBabe":
                    ModEntry.ShortMainBabe = true;
                    BtWalker.StripBabeBt(mainEnding);
                    return true;
                case "ShortNewBabe":
                    ModEntry.ShortNewBabe = true;
                    BtWalker.StripBabeBt(newEnding);
                    return true;
                case "ShortGhostBabe":
                    ModEntry.ShortGhostBabe = true;
                    BtWalker.StripBabeBt(ghostEnding);
                    return true;
            }

            return false;
        }

        public static bool HandleHitboxTag(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            var hitboxRegex =
                new Regex(@"^(Main|New|Ghost)BabeHitbox=\((\d+),\s*(\d+),\s*([1-9]\d*),\s*([1-9]\d*)\)$");
            var hitboxMatch = hitboxRegex.Match(tag);
            if (!hitboxMatch.Success)
            {
                return false;
            }

            var x = int.Parse(hitboxMatch.Groups[2].Value);
            var y = int.Parse(hitboxMatch.Groups[3].Value);
            var width = int.Parse(hitboxMatch.Groups[4].Value);
            var height = int.Parse(hitboxMatch.Groups[5].Value);
            var hitbox = new Rectangle(x, y, width, height);

            var screen = -1;
            switch (hitboxMatch.Groups[1].Value)
            {
                case "Main":
                    ModEntry.HitboxMainBabe = hitbox;
                    screen = mainEnding.GetEndingScreen0();
                    break;
                case "New":
                    ModEntry.HitboxNewBabe = hitbox;
                    screen = newEnding.GetEndingScreen0();
                    break;
                case "Ghost":
                    ModEntry.HitboxGhostBabe = hitbox;
                    screen = ghostEnding.GetEndingScreen0();
                    break;
            }

            _ = new EntityDebugHitbox(screen + 1, hitbox);
            return true;
        }

        public static bool HandleKingPositionTag(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            var positionRegex = new Regex(
                @"^(Main|New|Ghost)KingPosition=\((\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?)\)$");
            var positionMatch = positionRegex.Match(tag);
            if (!positionMatch.Success)
            {
                return false;
            }

            var x = float.Parse(positionMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            var y = float.Parse(positionMatch.Groups[3].Value, CultureInfo.InvariantCulture);

            switch (positionMatch.Groups[1].Value)
            {
                case "Main":
                    SetKingPosition(mainEnding,
                        new Vector2(x, y - (mainEnding.GetEndingScreen0() * 360)));
                    break;
                case "New":
                    SetKingPosition(newEnding,
                        new Vector2(x, y - (newEnding.GetEndingScreen0() * 360)));
                    break;
                case "Ghost":
                    SetKingPosition(ghostEnding,
                        new Vector2(x, y - (ghostEnding.GetEndingScreen0() * 360)));
                    break;
            }

            return true;

            void SetKingPosition(IEnding ending, Vector2 position) =>
                Traverse
                    .Create(ending)
                    .Field("endingKingStartPosition")
                    .SetValue(position);
        }

        public static bool HandleBabePositionTag(string tag, IEnding mainEnding, IEnding newEnding, IEnding ghostEnding)
        {
            var positionRegex = new Regex(
                @"^(Main|New|Ghost)BabePosition=\((\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?)\)$");
            var positionMatch = positionRegex.Match(tag);
            if (!positionMatch.Success)
            {
                return false;
            }

            var x = float.Parse(positionMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            var y = float.Parse(positionMatch.Groups[3].Value, CultureInfo.InvariantCulture);

            switch (positionMatch.Groups[1].Value)
            {
                case "Main":
                    SetBabePosition(mainEnding,
                        new Vector2(x, y - (mainEnding.GetEndingScreen0() * 360)));
                    break;
                case "New":
                    SetBabePosition(newEnding,
                        new Vector2(x, y - (newEnding.GetEndingScreen0() * 360)));
                    break;
                case "Ghost":
                    SetBabePosition(ghostEnding,
                        new Vector2(x, y - (ghostEnding.GetEndingScreen0() * 360)));
                    break;
            }

            return true;

            void SetBabePosition(IEnding ending, Vector2 position)
            {
                var babe = Traverse
                    .Create(ending)
                    .Field("m_babe")
                    .GetValue<ISpriteEntity>();
                babe.Position = position;
            }
        }

        public static bool HandleGargoylePositionTag(string tag, IEnding ghostEnding)
        {
            var positionRegex = new Regex(
                @"^GhostGargoylePosition=\((\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?)\)$");
            var positionMatch = positionRegex.Match(tag);
            if (!positionMatch.Success)
            {
                return false;
            }

            var x = float.Parse(positionMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            var y = float.Parse(positionMatch.Groups[2].Value, CultureInfo.InvariantCulture);

            var gargoyle = Traverse
                .Create(ghostEnding)
                .Field("m_gargoyle")
                .GetValue<ISpriteEntity>();
            gargoyle.Position = new Vector2(x, y - (ghostEnding.GetEndingScreen0() * 360));

            return true;
        }
    }
}
