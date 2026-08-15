using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.CanBeCaptured), MethodType.Getter)]
    public static class CompHoldingPlatformTarget_CanBeCaptured_Patch
    {
        public static void Postfix(CompHoldingPlatformTarget __instance, ref bool __result)
        {
            if (__result is false && __instance.parent is Pawn pawn && pawn.RaceProps.Humanlike && pawn.IsMutant is false && (pawn.Downed || pawn.ParentHolder is Pawn_CarryTracker))
            {
                __result = true;
            }
        }
    }
}