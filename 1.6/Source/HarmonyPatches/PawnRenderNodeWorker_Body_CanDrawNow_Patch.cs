using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Body), nameof(PawnRenderNodeWorker_Body.CanDrawNow))]
    public static class PawnRenderNodeWorker_Body_CanDrawNow_Patch
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