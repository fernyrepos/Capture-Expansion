using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed), "JobOnThing")]
    public static class WorkGiver_Warden_TakeToBed_JobOnThing_Patch
    {
        public static bool Prefix(Thing t, ref Job __result)
        {
            if (t is Pawn p && p.ParentHolder is Building_HoldingPlatform)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}
