using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.CompTick))]
    public static class CompHoldingPlatformTarget_CompTick_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var entityHolderProp = AccessTools.PropertyGetter(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.EntityHolder));

            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].Calls(entityHolderProp))
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = AccessTools.Method(typeof(CompHoldingPlatformTarget_CompTick_Patch), nameof(TargetHolderHeldPawn));
                    codes.RemoveAt(i + 1);
                    break;
                }
            }
            return codes;
        }

        public static Pawn TargetHolderHeldPawn(CompHoldingPlatformTarget comp)
        {
            if (comp.targetHolder is Building_Bed bed)
            {
                var validBed = bed.ForPrisoners || bed is Building_Cage;
                var canUse = bed.AnyUnownedSleepingSlot || bed.IsOwner(comp.parent as Pawn);
                return validBed is false || canUse is false ? comp.parent as Pawn : null;
            }
            return comp.EntityHolder?.HeldPawn;
        }
    }
}
