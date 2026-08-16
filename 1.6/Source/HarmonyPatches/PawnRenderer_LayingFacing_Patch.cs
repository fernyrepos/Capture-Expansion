using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing))]
    public static class PawnRenderer_LayingFacing_Patch
    {
        public static void Postfix(PawnRenderer __instance, ref Rot4 __result)
        {
            if (__instance.pawn.InBed() && State.IsRestrained(__instance.pawn))
            {
                __result = Rot4.South;
            }
        }
    }
}