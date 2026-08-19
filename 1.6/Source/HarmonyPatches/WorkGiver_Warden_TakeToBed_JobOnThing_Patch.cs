using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed), "JobOnThing")]
    public static class WorkGiver_Warden_TakeToBed_JobOnThing_Patch
    {
        public static bool Prefix(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (t is Pawn prisoner)
            {
                var comp = prisoner.TryGetComp<CompHoldingPlatformTarget>();
                var targetHolder = comp?.targetHolder;
                var currentPlatform = prisoner.ParentHolder as Building_HoldingPlatform;
                var isCurrentlyOnPlatform = currentPlatform != null;

                if (targetHolder is Building_Bed bed)
                {
                    if (prisoner.CurrentBed() == bed)
                    {
                        comp.targetHolder = null;
                        return true;
                    }

                    if (isCurrentlyOnPlatform)
                    {
                        if (pawn.CanReserveAndReach(prisoner, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Deadly, bed.SleepingSlotsCount, 0, null, forced))
                        {
                            var job = JobMaker.MakeJob(DefsOf.CE_TakeHeldPrisonerToBed, currentPlatform, bed, prisoner);
                            job.count = 1;
                            __result = job;
                        }
                        else
                        {
                            __result = null;
                        }
                        return false;
                    }
                    else
                    {
                        if (pawn.CanReserveAndReach(prisoner, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Deadly, bed.SleepingSlotsCount, 0, null, forced))
                        {
                            var def = prisoner.Downed ? JobDefOf.TakeWoundedPrisonerToBed : JobDefOf.EscortPrisonerToBed;
                            var job = JobMaker.MakeJob(def, prisoner, bed);
                            job.count = 1;
                            __result = job;
                        }
                        else
                        {
                            __result = null;
                        }
                        return false;
                    }
                }
                else if (targetHolder is Building_HoldingPlatform destPlatform && destPlatform != currentPlatform)
                {
                    if (isCurrentlyOnPlatform && pawn.CanReserveAndReach(prisoner, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserveAndReach(destPlatform, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced))
                    {
                        var job = JobMaker.MakeJob(JobDefOf.TransferBetweenEntityHolders, currentPlatform, destPlatform, prisoner);
                        job.count = 1;
                        __result = job;
                        return false;
                    }
                    __result = null;
                    return false;
                }

                if (isCurrentlyOnPlatform)
                {
                    __result = null;
                    return false;
                }
            }
            return true;
        }
    }
}
