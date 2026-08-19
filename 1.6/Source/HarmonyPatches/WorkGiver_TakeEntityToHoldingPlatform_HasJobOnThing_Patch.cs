using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_TakeEntityToHoldingPlatform), nameof(WorkGiver_TakeEntityToHoldingPlatform.HasJobOnThing))]
    public static class WorkGiver_TakeEntityToHoldingPlatform_HasJobOnThing_Patch
    {
        public static bool Prefix(Thing t, ref bool __result)
        {
            var comp = t.TryGetComp<CompHoldingPlatformTarget>();
            if (comp?.targetHolder != null && comp.targetHolder is not Building_HoldingPlatform)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
