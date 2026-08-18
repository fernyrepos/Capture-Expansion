using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(RestUtility), nameof(RestUtility.InBed))]
    public static class RestUtility_InBed_Patch
    {
        public static int forWardenJob;

        public static void Postfix(Pawn p, ref bool __result)
        {
            if (__result is false && forWardenJob > 0 && p.RaceProps.Humanlike && p.ParentHolder is Building_HoldingPlatform)
            {
                __result = true;
            }
        }
    }
}
