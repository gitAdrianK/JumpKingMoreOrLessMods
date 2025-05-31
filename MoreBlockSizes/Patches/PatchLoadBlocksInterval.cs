namespace MoreBlockSizes.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JumpKing.Level;
    using JumpKing.Level.Sampler;
    using Microsoft.Xna.Framework;

    [HarmonyPatch(typeof(LevelManager), "LoadBlocksInterval")]
    public class PatchLoadBlocksInterval
    {
        public static LevelTexture Sizes { get; set; } = null;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            var firstBrReplaceIndex = -1;
            var firstContinueFound = false;
            var firstContinueLabel = il.DefineLabel();

            var secondBrReplaceIndex = -1;
            var secondContinueFound = false;
            var secondContinueLabel = il.DefineLabel();

            // br.s and a later brfalse.s share the same jump index.
            var brsReplaceIndex = -1;
            var thirdContinueFound = false;
            var thirdContinueLabel = il.DefineLabel();

            var firstInsertionIndex = -1;

            var removalIndex = -1;

            var secondInsertionIndex = -1;

            var rectangleCtor = AccessTools.Constructor(
                typeof(Rectangle),
                new Type[] { typeof(Point), typeof(Point) });

            int i;
            // Find the two br instructions that need to be adjusted.
            for (i = 0; i < code.Count - 3; i++)
            {
                if (code[i].opcode == OpCodes.Br
                    && code[i + 1].opcode == OpCodes.Ldc_I4_0
                    && code[i + 2].opcode == OpCodes.Stloc_S
                    && code[i + 3].opcode == OpCodes.Br)
                {
                    firstBrReplaceIndex = i;
                    secondBrReplaceIndex = i + 3;
                    break;
                }
            }
            // Find the br.s instruction that needs to be adjusted.
            for (; i < code.Count - 2; i++)
            {
                if (code[i].opcode == OpCodes.Stind_I1
                    && code[i + 1].opcode == OpCodes.Br_S
                    && code[i + 2].opcode == OpCodes.Ldloca_S)
                {
                    brsReplaceIndex = i + 1;
                    break;
                }
            }
            // Insertion index of our method call.
            for (; i < code.Count - 2; i++)
            {
                if (code[i].opcode == OpCodes.Ldloc_S
                    && code[i + 1].opcode == OpCodes.Call
                    && (code[i + 1].operand as ConstructorInfo) == rectangleCtor
                    && code[i + 2].opcode == OpCodes.Ldloc_S)
                {
                    firstInsertionIndex = i + 2;
                    break;
                }
            }
            // Removal index, we are removing all 3 btw.
            // Also insertion index as we are inserting right after.
            for (; i < code.Count - 4; i++)
            {
                if (code[i].opcode == OpCodes.Ldloc_S
                    && (code[i].operand as LocalBuilder).LocalIndex == 11
                    && code[i + 1].opcode == OpCodes.Brfalse_S
                    && code[i + 2].opcode == OpCodes.Ldloc_3
                    && code[i + 3].opcode == OpCodes.Ldloc_S
                    && (code[i + 3].operand as LocalBuilder).LocalIndex == 11
                    && code[i + 4].opcode == OpCodes.Callvirt)
                {
                    removalIndex = i;
                    secondInsertionIndex = i + 4;
                    break;
                }
            }
            // Third jump label comes first
            for (; i < code.Count - 3; i++)
            {
                if (code[i].opcode == OpCodes.Callvirt
                    && code[i + 1].opcode == OpCodes.Ldloc_S
                    && code[i + 2].opcode == OpCodes.Ldc_I4_1
                    && code[i + 3].opcode == OpCodes.Add)
                {
                    thirdContinueFound = true;
                    code[i + 1].labels.Add(thirdContinueLabel);
                    break;
                }
            }
            // Then second label comes first
            for (; i < code.Count - 3; i++)
            {
                if (code[i].opcode == OpCodes.Stloc_S
                    && code[i + 1].opcode == OpCodes.Ldloc_S
                    && code[i + 2].opcode == OpCodes.Ldloc_1
                    && code[i + 3].opcode == OpCodes.Blt)
                {
                    secondContinueFound = true;
                    code[i + 1].labels.Add(secondContinueLabel);
                    break;
                }
            }
            // And lastly first
            for (; i < code.Count - 4; i++)
            {
                if (code[i].opcode == OpCodes.Add
                    && code[i + 1].opcode == OpCodes.Stloc_S
                    && code[i + 2].opcode == OpCodes.Ldloc_S
                    && code[i + 3].opcode == OpCodes.Ldloc_2
                    && code[i + 4].opcode == OpCodes.Blt)
                {
                    firstContinueFound = true;
                    code[i + 2].labels.Add(firstContinueLabel);
                    break;
                }
            }

            if (firstBrReplaceIndex == -1
                || secondBrReplaceIndex == -1
                || brsReplaceIndex == -1
                || firstInsertionIndex == -1
                || secondInsertionIndex == -1
                || removalIndex == -1
                || !firstContinueFound
                || !secondContinueFound
                || !thirdContinueFound)
            {
                return code.AsEnumerable();
            }

            var firstInsert = new List<CodeInstruction>
            {
                new CodeInstruction(
                    OpCodes.Ldarg_2),
                new CodeInstruction(
                    OpCodes.Ldloc_S, 5),
                new CodeInstruction(
                    OpCodes.Ldloc_S, 4),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(PatchLoadBlocksInterval),
                        nameof(GetHitbox),
                        new Type[] { typeof(int), typeof(int), typeof(int) })),
                new CodeInstruction(
                    OpCodes.Stloc_S, 10)
            };

            var secondInsert = new List<CodeInstruction>
            {
                new CodeInstruction(
                    OpCodes.Brfalse_S, thirdContinueLabel),
                new CodeInstruction(
                    OpCodes.Ldloc_3),
                new CodeInstruction(
                    OpCodes.Ldloc_S, 11)
            };

            code[firstBrReplaceIndex] = new CodeInstruction(OpCodes.Br, firstContinueLabel);
            code[secondBrReplaceIndex] = new CodeInstruction(OpCodes.Br, secondContinueLabel);
            code[brsReplaceIndex] = new CodeInstruction(OpCodes.Br_S, thirdContinueLabel);
            var adjust = 0;
            code.InsertRange(firstInsertionIndex, firstInsert);
            adjust += firstInsert.Count;
            code.RemoveRange(removalIndex + adjust, 3);
            adjust -= 3;
            code.InsertRange(secondInsertionIndex + adjust, secondInsert);

            return code.AsEnumerable();
        }

        public static Rectangle GetHitbox(int screen, int x, int y)
        {
            var location = new Point(x * 8, (y - (screen * 45)) * 8);
            var size = new Point(8, 8);
            if (Sizes == null)
            {
                return new Rectangle(location, size);
            }

            var color = Sizes.GetColor(screen, x, y);
            if (color.R > 63
                || color.G < 1
                || color.B < 1)
            {
                return new Rectangle(location, size);
            }

            var mod = color.R % 8;
            var div = color.R / 8;
            return new Rectangle(
                location.X + mod,
                location.Y + div,
                Math.Min(color.G, 8 - mod),
                Math.Min(color.B, 8 - div));
        }
    }
}

/*
It's a lot so just throw this into a text comparer

.method assembly hidebysig static
class JumpKing.Level.IBlock[] LoadBlocksInterval (
class JumpKing.Level.Sampler.LevelTexture p_src,
class JumpKing.Workshop.Level level,
int32 p_screen,
[out] bool& wind_enabled,
[out] class JumpKing.Level.TeleportLink[]& teleport,
[out] float32& wind_int,
[out] valuetype [mscorlib]System.Nullable`1<bool>& wind_direction
) cil managed
{
// Header Size: 12 bytes
// Code Size: 634 (0x27A) bytes
// LocalVarSig Token: 0x110001A3 RID: 419
.maxstack 8
.locals init (
[0] class JumpKing.Level.LevelManager/'<>c__DisplayClass30_0',
[1] int32 num,
[2] int32 num2,
[3] class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock> list,
[4] int32 i,
[5] int32 j,
[6] class JumpKing.Level.LevelManager/'<>c__DisplayClass30_1',
[7] class JumpKing.API.IBlockFactory blockFactory,
[8] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point location,
[9] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point size,
[10] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle blockRect,
[11] class JumpKing.Level.IBlock block
)
IL_0000: newobj    instance void JumpKing.Level.LevelManager/'<>c__DisplayClass30_0'::.ctor()
IL_0005: stloc.0
IL_0006: ldloc.0
IL_0007: ldarg.1
IL_0008: stfld     class JumpKing.Workshop.Level JumpKing.Level.LevelManager/'<>c__DisplayClass30_0'::level
IL_000D: ldarg.3
IL_000E: ldc.i4.0
IL_000F: stind.i1
IL_0010: ldarg.s   wind_int
IL_0012: ldc.r4    0.0
IL_0017: stind.r4
IL_0018: ldarg.s   wind_direction
IL_001A: initobj   valuetype [mscorlib]System.Nullable`1<bool>
IL_0020: ldarg.s   teleport
IL_0022: ldc.i4.2
IL_0023: newarr    JumpKing.Level.TeleportLink
IL_0028: stind.ref
IL_0029: ldarg.s   teleport
IL_002B: ldind.ref
IL_002C: ldc.i4.0
IL_002D: newobj    instance void JumpKing.Level.TeleportLink::.ctor()
IL_0032: stelem.ref
IL_0033: ldarg.s   teleport
IL_0035: ldind.ref
IL_0036: ldc.i4.1
IL_0037: newobj    instance void JumpKing.Level.TeleportLink::.ctor()
IL_003C: stelem.ref
IL_003D: ldc.i4.s  60
IL_003F: stloc.1
IL_0040: ldc.i4.s  45
IL_0042: stloc.2
IL_0043: newobj    instance void class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::.ctor()
IL_0048: stloc.3
IL_0049: ldc.i4.0
IL_004A: stloc.s   i
IL_004C: br        IL_026B
// loop start (head: IL_026B)
IL_0051: ldc.i4.0
IL_0052: stloc.s   j
IL_0054: br        IL_025D
// loop start (head: IL_025D)
IL_0059: newobj    instance void JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::.ctor()
IL_005E: stloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0060: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0062: ldloc.0
IL_0063: stfld     class JumpKing.Level.LevelManager/'<>c__DisplayClass30_0' JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::'CS$<>8__locals1'
IL_0068: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_006A: ldarg.0
IL_006B: ldarg.2
IL_006C: ldloc.s   j
IL_006E: ldloc.s   i
IL_0070: callvirt  instance valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.Sampler.LevelTexture::GetColor(int32, int32, int32)
IL_0075: stfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_007A: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_007C: ldfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0081: ldsfld    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager::BLOCKCODE_WIND
IL_0086: call      bool [MonoGame.Framework]Microsoft.Xna.Framework.Color::op_Equality(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color)
IL_008B: brfalse.s IL_0090
IL_008D: ldarg.3
IL_008E: ldc.i4.1
IL_008F: stind.i1
IL_0090: ldsfld    class [mscorlib]System.Collections.Generic.List`1<class JumpKing.API.IBlockFactory> JumpKing.Level.LevelManager::BlockFactories
IL_0095: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0097: ldftn     instance bool JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::'<LoadBlocksInterval>b__0'(class JumpKing.API.IBlockFactory)
IL_009D: newobj    instance void class [mscorlib]System.Func`2<class JumpKing.API.IBlockFactory, bool>::.ctor(object, native int)
IL_00A2: call      !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class JumpKing.API.IBlockFactory>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
IL_00A7: stloc.s   blockFactory
IL_00A9: ldloc.s   blockFactory
IL_00AB: brtrue    IL_01FF
IL_00B0: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_00B2: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_00B7: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_G()
IL_00BC: brtrue.s  IL_0105
IL_00BE: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_00C0: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_00C5: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_B()
IL_00CA: ldc.i4    255
IL_00CF: bne.un.s  IL_0105
IL_00D1: ldloc.s   j
IL_00D3: ldc.i4.s  30
IL_00D5: blt.s     IL_00EF
IL_00D7: ldarg.s   teleport
IL_00D9: ldind.ref
IL_00DA: ldc.i4.1
IL_00DB: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_00DD: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_00E2: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_00E7: newobj    instance void JumpKing.Level.TeleportLink::.ctor(int32)
IL_00EC: stelem.ref
IL_00ED: br.s      IL_0105
IL_00EF: ldarg.s   teleport
IL_00F1: ldind.ref
IL_00F2: ldc.i4.0
IL_00F3: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_00F5: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_00FA: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_00FF: newobj    instance void JumpKing.Level.TeleportLink::.ctor(int32)
IL_0104: stelem.ref
IL_0105: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0107: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_010C: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_G()
IL_0111: ldc.i4    255
IL_0116: bne.un    IL_0257
IL_011B: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_011D: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0122: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_B()
IL_0127: brtrue    IL_0257
IL_012C: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_012E: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0133: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0138: ldc.i4    208
IL_013D: bge.s     IL_0168
IL_013F: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0141: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0146: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_014B: ldc.i4    191
IL_0150: ble.s     IL_0168
IL_0152: ldarg.s   wind_int
IL_0154: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0156: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_015B: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0160: ldc.i4    191
IL_0165: sub
IL_0166: conv.r4
IL_0167: stind.r4
IL_0168: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_016A: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_016F: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0174: ldc.i4    207
IL_0179: ble.s     IL_01B1
IL_017B: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_017D: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0182: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0187: ldc.i4    224
IL_018C: bge.s     IL_01B1
IL_018E: ldarg.s   wind_int
IL_0190: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0192: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0197: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_019C: ldc.i4    207
IL_01A1: sub
IL_01A2: conv.r4
IL_01A3: stind.r4
IL_01A4: ldarg.s   wind_direction
IL_01A6: ldc.i4.1
IL_01A7: newobj    instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
IL_01AC: stobj     valuetype [mscorlib]System.Nullable`1<bool>
IL_01B1: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_01B3: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_01B8: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01BD: ldc.i4    223
IL_01C2: ble.s     IL_01FA
IL_01C4: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_01C6: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_01CB: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01D0: ldc.i4    240
IL_01D5: bge.s     IL_01FA
IL_01D7: ldarg.s   wind_int
IL_01D9: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_01DB: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_01E0: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01E5: ldc.i4    223
IL_01EA: sub
IL_01EB: conv.r4
IL_01EC: stind.r4
IL_01ED: ldarg.s   wind_direction
IL_01EF: ldc.i4.0
IL_01F0: newobj    instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
IL_01F5: stobj     valuetype [mscorlib]System.Nullable`1<bool>
IL_01FA: ldarg.3
IL_01FB: ldc.i4.1
IL_01FC: stind.i1
IL_01FD: br.s      IL_0257
IL_01FF: ldloca.s  location
IL_0201: ldloc.s   j
IL_0203: ldc.i4.8
IL_0204: mul
IL_0205: ldloc.s   i
IL_0207: ldarg.2
IL_0208: ldc.i4.s  45
IL_020A: mul
IL_020B: sub
IL_020C: ldc.i4.8
IL_020D: mul
IL_020E: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Point::.ctor(int32, int32)
IL_0213: ldloca.s  size
IL_0215: ldc.i4.8
IL_0216: ldc.i4.8
IL_0217: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Point::.ctor(int32, int32)
IL_021C: ldloca.s  blockRect
IL_021E: ldloc.s   location
IL_0220: ldloc.s   size
IL_0222: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle::.ctor(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point)
IL_0227: ldloc.s   blockFactory
IL_0229: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_022B: ldfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::color
IL_0230: ldloc.s   blockRect
IL_0232: ldloc.s   JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'
IL_0234: ldfld     class JumpKing.Level.LevelManager/'<>c__DisplayClass30_0' JumpKing.Level.LevelManager/'<>c__DisplayClass30_1'::'CS$<>8__locals1'
IL_0239: ldfld     class JumpKing.Workshop.Level JumpKing.Level.LevelManager/'<>c__DisplayClass30_0'::level
IL_023E: ldarg.0
IL_023F: ldarg.2
IL_0240: ldloc.s   j
IL_0242: ldloc.s   i
IL_0244: callvirt  instance class JumpKing.Level.IBlock JumpKing.API.IBlockFactory::GetBlock(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle, class JumpKing.Workshop.Level, class JumpKing.Level.Sampler.LevelTexture, int32, int32, int32)
IL_0249: stloc.s   block
IL_024B: ldloc.s   block
IL_024D: brfalse.s IL_0257
IL_024F: ldloc.3
IL_0250: ldloc.s   block
IL_0252: callvirt  instance void class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::Add(!0)
IL_0257: ldloc.s   j
IL_0259: ldc.i4.1
IL_025A: add
IL_025B: stloc.s   j
IL_025D: ldloc.s   j
IL_025F: ldloc.1
IL_0260: blt       IL_0059
// end loop
IL_0265: ldloc.s   i
IL_0267: ldc.i4.1
IL_0268: add
IL_0269: stloc.s   i
IL_026B: ldloc.s   i
IL_026D: ldloc.2
IL_026E: blt       IL_0051
// end loop
IL_0273: ldloc.3
IL_0274: callvirt  instance !0[] class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::ToArray()
IL_0279: ret
} // end of method LevelManager::LoadBlocksInterval

.method assembly hidebysig static
class JumpKing.Level.IBlock[] LoadBlocksInterval (
class JumpKing.Level.Sampler.LevelTexture p_src,
class JumpKing.Workshop.Level level,
int32 p_screen,
[out] bool& wind_enabled,
[out] class JumpKing.Level.TeleportLink[]& teleport,
[out] float32& wind_int,
[out] valuetype [mscorlib]System.Nullable`1<bool>& wind_direction
) cil managed
{
// Header Size: 12 bytes
// Code Size: 646 (0x286) bytes
// LocalVarSig Token: 0x11000001 RID: 1
.maxstack 8
.locals init (
[0] class JumpKing.Level.LevelManager/'<>c__DisplayClass0_0' 'CS$<>8__locals0',
[1] int32 num,
[2] int32 num2,
[3] class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock> list,
[4] int32 i,
[5] int32 j,
[6] class JumpKing.Level.LevelManager/'<>c__DisplayClass0_1' 'CS$<>8__locals1',
[7] class JumpKing.API.IBlockFactory blockFactory,
[8] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point location,
[9] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point size,
[10] valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle blockRect,
[11] class JumpKing.Level.IBlock block
)
IL_0000: newobj    instance void JumpKing.Level.LevelManager/'<>c__DisplayClass0_0'::.ctor()
IL_0005: stloc.0
IL_0006: ldloc.0
IL_0007: ldarg.1
IL_0008: stfld     class JumpKing.Workshop.Level JumpKing.Level.LevelManager/'<>c__DisplayClass0_0'::level
IL_000D: ldarg.3
IL_000E: ldc.i4.0
IL_000F: stind.i1
IL_0010: ldarg.s   wind_int
IL_0012: ldc.r4    0.0
IL_0017: stind.r4
IL_0018: ldarg.s   wind_direction
IL_001A: initobj   valuetype [mscorlib]System.Nullable`1<bool>
IL_0020: ldarg.s   teleport
IL_0022: ldc.i4.2
IL_0023: newarr    JumpKing.Level.TeleportLink
IL_0028: stind.ref
IL_0029: ldarg.s   teleport
IL_002B: ldind.ref
IL_002C: ldc.i4.0
IL_002D: newobj    instance void JumpKing.Level.TeleportLink::.ctor()
IL_0032: stelem.ref
IL_0033: ldarg.s   teleport
IL_0035: ldind.ref
IL_0036: ldc.i4.1
IL_0037: newobj    instance void JumpKing.Level.TeleportLink::.ctor()
IL_003C: stelem.ref
IL_003D: ldc.i4.s  60
IL_003F: stloc.1
IL_0040: ldc.i4.s  45
IL_0042: stloc.2
IL_0043: newobj    instance void class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::.ctor()
IL_0048: stloc.3
IL_0049: ldc.i4.0
IL_004A: stloc.s   i
IL_004C: br        IL_0277
// loop start (head: IL_0277)
IL_0051: ldc.i4.0
IL_0052: stloc.s   j
IL_0054: br        IL_0269
// loop start (head: IL_0269)
IL_0059: newobj    instance void JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::.ctor()
IL_005E: stloc.s   'CS$<>8__locals1'
IL_0060: ldloc.s   'CS$<>8__locals1'
IL_0062: ldloc.0
IL_0063: stfld     class JumpKing.Level.LevelManager/'<>c__DisplayClass0_0' JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::'CS$<>8__locals1'
IL_0068: ldloc.s   'CS$<>8__locals1'
IL_006A: ldarg.0
IL_006B: ldarg.2
IL_006C: ldloc.s   j
IL_006E: ldloc.s   i
IL_0070: callvirt  instance valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.Sampler.LevelTexture::GetColor(int32, int32, int32)
IL_0075: stfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_007A: ldloc.s   'CS$<>8__locals1'
IL_007C: ldfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0081: ldsfld    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager::BLOCKCODE_WIND
IL_0086: call      bool [MonoGame.Framework]Microsoft.Xna.Framework.Color::op_Equality(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color)
IL_008B: brfalse.s IL_0090
IL_008D: ldarg.3
IL_008E: ldc.i4.1
IL_008F: stind.i1
IL_0090: ldsfld    class [mscorlib]System.Collections.Generic.List`1<class JumpKing.API.IBlockFactory> JumpKing.Level.LevelManager::BlockFactories
IL_0095: ldloc.s   'CS$<>8__locals1'
IL_0097: ldftn     instance bool JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::'<LoadBlocksInterval>b__0'(class JumpKing.API.IBlockFactory)
IL_009D: newobj    instance void class [mscorlib]System.Func`2<class JumpKing.API.IBlockFactory, bool>::.ctor(object, native int)
IL_00A2: call      !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class JumpKing.API.IBlockFactory>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
IL_00A7: stloc.s   blockFactory
IL_00A9: ldloc.s   blockFactory
IL_00AB: brtrue    IL_01FF
IL_00B0: ldloc.s   'CS$<>8__locals1'
IL_00B2: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_00B7: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_G()
IL_00BC: brtrue.s  IL_0105
IL_00BE: ldloc.s   'CS$<>8__locals1'
IL_00C0: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_00C5: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_B()
IL_00CA: ldc.i4    255
IL_00CF: bne.un.s  IL_0105
IL_00D1: ldloc.s   j
IL_00D3: ldc.i4.s  30
IL_00D5: blt.s     IL_00EF
IL_00D7: ldarg.s   teleport
IL_00D9: ldind.ref
IL_00DA: ldc.i4.1
IL_00DB: ldloc.s   'CS$<>8__locals1'
IL_00DD: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_00E2: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_00E7: newobj    instance void JumpKing.Level.TeleportLink::.ctor(int32)
IL_00EC: stelem.ref
IL_00ED: br.s      IL_0105
IL_00EF: ldarg.s   teleport
IL_00F1: ldind.ref
IL_00F2: ldc.i4.0
IL_00F3: ldloc.s   'CS$<>8__locals1'
IL_00F5: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_00FA: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_00FF: newobj    instance void JumpKing.Level.TeleportLink::.ctor(int32)
IL_0104: stelem.ref
IL_0105: ldloc.s   'CS$<>8__locals1'
IL_0107: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_010C: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_G()
IL_0111: ldc.i4    255
IL_0116: bne.un    IL_0263
IL_011B: ldloc.s   'CS$<>8__locals1'
IL_011D: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0122: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_B()
IL_0127: brtrue    IL_0263
IL_012C: ldloc.s   'CS$<>8__locals1'
IL_012E: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0133: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0138: ldc.i4    208
IL_013D: bge.s     IL_0168
IL_013F: ldloc.s   'CS$<>8__locals1'
IL_0141: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0146: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_014B: ldc.i4    191
IL_0150: ble.s     IL_0168
IL_0152: ldarg.s   wind_int
IL_0154: ldloc.s   'CS$<>8__locals1'
IL_0156: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_015B: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0160: ldc.i4    191
IL_0165: sub
IL_0166: conv.r4
IL_0167: stind.r4
IL_0168: ldloc.s   'CS$<>8__locals1'
IL_016A: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_016F: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0174: ldc.i4    207
IL_0179: ble.s     IL_01B1
IL_017B: ldloc.s   'CS$<>8__locals1'
IL_017D: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0182: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_0187: ldc.i4    224
IL_018C: bge.s     IL_01B1
IL_018E: ldarg.s   wind_int
IL_0190: ldloc.s   'CS$<>8__locals1'
IL_0192: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_0197: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_019C: ldc.i4    207
IL_01A1: sub
IL_01A2: conv.r4
IL_01A3: stind.r4
IL_01A4: ldarg.s   wind_direction
IL_01A6: ldc.i4.1
IL_01A7: newobj    instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
IL_01AC: stobj     valuetype [mscorlib]System.Nullable`1<bool>
IL_01B1: ldloc.s   'CS$<>8__locals1'
IL_01B3: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_01B8: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01BD: ldc.i4    223
IL_01C2: ble.s     IL_01FA
IL_01C4: ldloc.s   'CS$<>8__locals1'
IL_01C6: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_01CB: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01D0: ldc.i4    240
IL_01D5: bge.s     IL_01FA
IL_01D7: ldarg.s   wind_int
IL_01D9: ldloc.s   'CS$<>8__locals1'
IL_01DB: ldflda    valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_01E0: call      instance uint8 [MonoGame.Framework]Microsoft.Xna.Framework.Color::get_R()
IL_01E5: ldc.i4    223
IL_01EA: sub
IL_01EB: conv.r4
IL_01EC: stind.r4
IL_01ED: ldarg.s   wind_direction
IL_01EF: ldc.i4.0
IL_01F0: newobj    instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
IL_01F5: stobj     valuetype [mscorlib]System.Nullable`1<bool>
IL_01FA: ldarg.3
IL_01FB: ldc.i4.1
IL_01FC: stind.i1
IL_01FD: br.s      IL_0263
IL_01FF: ldloca.s  location
IL_0201: ldloc.s   j
IL_0203: ldc.i4.8
IL_0204: mul
IL_0205: ldloc.s   i
IL_0207: ldarg.2
IL_0208: ldc.i4.s  45
IL_020A: mul
IL_020B: sub
IL_020C: ldc.i4.8
IL_020D: mul
IL_020E: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Point::.ctor(int32, int32)
IL_0213: ldloca.s  size
IL_0215: ldc.i4.8
IL_0216: ldc.i4.8
IL_0217: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Point::.ctor(int32, int32)
IL_021C: ldloca.s  blockRect
IL_021E: ldloc.s   location
IL_0220: ldloc.s   size
IL_0222: call      instance void [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle::.ctor(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Point)
IL_0227: ldarg.2
IL_0228: ldloc.s   j
IL_022A: ldloc.s   i
IL_022C: call      valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle [MoreBlockSizes]MoreBlockSizes.Patches.PatchLevelManager::GetHitbox(int32, int32, int32)
IL_0231: stloc.s   blockRect
IL_0233: ldloc.s   blockFactory
IL_0235: ldloc.s   'CS$<>8__locals1'
IL_0237: ldfld     valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::color
IL_023C: ldloc.s   blockRect
IL_023E: ldloc.s   'CS$<>8__locals1'
IL_0240: ldfld     class JumpKing.Level.LevelManager/'<>c__DisplayClass0_0' JumpKing.Level.LevelManager/'<>c__DisplayClass0_1'::'CS$<>8__locals1'
IL_0245: ldfld     class JumpKing.Workshop.Level JumpKing.Level.LevelManager/'<>c__DisplayClass0_0'::level
IL_024A: ldarg.0
IL_024B: ldarg.2
IL_024C: ldloc.s   j
IL_024E: ldloc.s   i
IL_0250: callvirt  instance class JumpKing.Level.IBlock JumpKing.API.IBlockFactory::GetBlock(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Color, valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle, class JumpKing.Workshop.Level, class JumpKing.Level.Sampler.LevelTexture, int32, int32, int32)
IL_0255: stloc.s   block
IL_0257: ldloc.s   block
IL_0259: brfalse.s IL_0263
IL_025B: ldloc.3
IL_025C: ldloc.s   block
IL_025E: callvirt  instance void class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::Add(!0)
IL_0263: ldloc.s   j
IL_0265: ldc.i4.1
IL_0266: add
IL_0267: stloc.s   j
IL_0269: ldloc.s   j
IL_026B: ldloc.1
IL_026C: blt       IL_0059
// end loop
IL_0271: ldloc.s   i
IL_0273: ldc.i4.1
IL_0274: add
IL_0275: stloc.s   i
IL_0277: ldloc.s   i
IL_0279: ldloc.2
IL_027A: blt       IL_0051
// end loop
IL_027F: ldloc.3
IL_0280: callvirt  instance !0[] class [mscorlib]System.Collections.Generic.List`1<class JumpKing.Level.IBlock>::ToArray()
IL_0285: ret
} // end of method LevelManager::LoadBlocksInterval

*/
