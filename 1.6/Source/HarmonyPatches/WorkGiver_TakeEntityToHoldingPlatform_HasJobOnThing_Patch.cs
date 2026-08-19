using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_TakeEntityToHoldingPlatform), nameof(WorkGiver_TakeEntityToHoldingPlatform.HasJobOnThing))]
    public static class WorkGiver_TakeEntityToHoldingPlatform_HasJobOnThing_Patch
    {
        public static void Postfix(Thing t, ref bool __result)
        {
            if (__result && t is Pawn { IsPrisonerOfColony: true })
            {
                __result = false;
            }
        }
    }
}
