using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_ReleasePrisoner), nameof(WorkGiver_Warden_ReleasePrisoner.JobOnThing))]
    public static class WorkGiver_Warden_ReleasePrisoner_JobOnThing_Patch
    {
        public static bool Prefix(WorkGiver_Warden_ReleasePrisoner __instance, Pawn pawn, Thing t, ref Job __result)
        {
            if (t is not Pawn prisoner || prisoner.ParentHolder is not Building_HoldingPlatform platform)
            {
                return true;
            }

            if (prisoner.IsPrisonerOfColony is false || prisoner.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Release) is false || prisoner.guest.Released || prisoner.InMentalState)
            {
                return true;
            }

            if (pawn.Faction != prisoner.Faction && pawn.MapHeld.CanEverExit is false)
            {
                JobFailReason.Is("CannotExitMap".Translate());
                __result = null;
                return false;
            }

            if (RCellFinder.TryFindPrisonerReleaseCell(prisoner, pawn, out var result) is false)
            {
                __result = null;
                return false;
            }

            var job = JobMaker.MakeJob(JobDefOf.ReleasePrisoner, prisoner, result);
            job.count = 1;
            __result = job;
            return false;
        }
    }
}
