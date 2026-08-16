using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Apparel_Head), nameof(PawnRenderNodeWorker_Apparel_Head.HeadgearVisible))]
    public static class PawnRenderNodeWorker_Apparel_Head_HeadgearVisible_Patch
    {
        public static void Postfix(PawnDrawParms parms, ref bool __result)
        {
            if (__result is false && parms.bed != null && State.IsRestrained(parms.pawn))
            {
                __result = true;
            }
        }
    }
}