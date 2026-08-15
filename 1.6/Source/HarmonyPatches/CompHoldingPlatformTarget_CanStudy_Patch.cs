using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), "CanStudy", MethodType.Getter)]
    public static class CompHoldingPlatformTarget_CanStudy_Patch
    {
        public static void Postfix(CompHoldingPlatformTarget __instance, ref bool __result)
        {
            if (__result && __instance.parent is Pawn pawn && pawn.RaceProps.Humanlike && pawn.IsMutant is false)
            {
                __result = false;
            }
        }
    }
}
