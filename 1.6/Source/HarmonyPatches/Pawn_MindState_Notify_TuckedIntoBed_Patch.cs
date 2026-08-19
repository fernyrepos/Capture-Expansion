using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn_MindState), nameof(Pawn_MindState.Notify_TuckedIntoBed))]
    public static class Pawn_MindState_Notify_TuckedIntoBed_Patch
    {
        public static void Postfix(Pawn_MindState __instance)
        {
            var comp = __instance.pawn.TryGetComp<CompHoldingPlatformTarget>();
            if (comp != null && comp.targetHolder is Building_Bed)
            {
                comp.targetHolder = null;
            }
        }
    }
}
