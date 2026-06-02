using System.Collections.Generic;
using System.Reflection.Emit;
using BitzArt.Nickelback.Mechanics;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BitzArt.Nickelback;

[HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "IsGettingCooled")]
internal static class QuenchBreakRecoveryPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var itemstackSetter = AccessTools.PropertySetter(typeof(ItemSlot), nameof(ItemSlot.Itemstack));
        var onQuenchBreak = AccessTools.Method(typeof(QuenchBreakRecoveryPatch), nameof(OnQuenchBreak));

        return new CodeMatcher(instructions)
            .MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(OpCodes.Ldnull),
                new CodeMatch(instruction => instruction.Calls(itemstackSetter))
            )
            .ThrowIfInvalid("Could not find quench item break point.")
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Call, onQuenchBreak)
            )
            .InstructionEnumeration();
    }

    private static void OnQuenchBreak(
        CollectibleBehaviorQuenchable quenchableBehavior,
        IWorldAccessor world,
        ItemSlot slot,
        Vec3d pos)
    {
        QuenchBreakRecoveryMechanic.RecoverBits(quenchableBehavior, world, slot, pos);
    }
}
