using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden), "ShouldTakeCareOfPrisoner")]
    public static class WorkGiver_Warden_ShouldTakeCareOfPrisoner_Patch
    {
        public static void Postfix(Pawn warden, Thing prisoner, bool forced, ref bool __result)
        {
            if (__result || prisoner is not Pawn p || p.ParentHolder is not Building_HoldingPlatform platform)
            {
                return;
            }
            var canReserveAndReach = warden.CanReserveAndReach(platform, PathEndMode.ClosestTouch, warden.NormalMaxDanger(), 1, -1, null, forced) && warden.CanReserve(p, 1, -1, null, forced);
            if (p.IsPrisonerOfColony is false || p.guest.PrisonerIsSecure is false || p.InAggroMentalState || prisoner.IsForbidden(warden) || p.IsFormingCaravan() || canReserveAndReach is false)
            {
                return;
            }
            __result = true;
        }
    }
}
