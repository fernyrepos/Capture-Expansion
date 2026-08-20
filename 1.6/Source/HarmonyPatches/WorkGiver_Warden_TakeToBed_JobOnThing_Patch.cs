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
            if (t is Building_HoldingPlatform platform && platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false, IsPrisonerOfColony: true } heldPrisoner)
            {
                var targetHolder = heldPrisoner.TryGetComp<CompHoldingPlatformTarget>()?.targetHolder;
                if (targetHolder is Building_Bed bed)
                {
                    var validBed = bed.Spawned && bed.IsBurning() is false && (bed is Building_Cage || bed.ForPrisoners) && RestUtility.CanUseBedEver(heldPrisoner, bed.def) && (bed.AnyUnownedSleepingSlot || bed.IsOwner(heldPrisoner));
                    var canResPlatform = pawn.CanReserveAndReach(platform, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced);
                    var canResBed = pawn.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Deadly, bed.SleepingSlotsCount, 0, null, forced);
                    var canResHeld = pawn.CanReserve(heldPrisoner, 1, -1, null, forced);
                    if (validBed && canResPlatform && canResBed && canResHeld)
                    {
                        var job = JobMaker.MakeJob(DefsOf.CE_TakeHeldPrisonerToBed, platform, bed, heldPrisoner);
                        job.count = 1;
                        __result = job;
                    }
                    else
                    {
                        __result = null;
                    }
                    return false;
                }
                else if (targetHolder is Building_HoldingPlatform destPlatform && destPlatform != platform)
                {
                    if (pawn.CanReserveAndReach(platform, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserveAndReach(destPlatform, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserve(heldPrisoner, 1, -1, null, forced))
                    {
                        var job = JobMaker.MakeJob(JobDefOf.TransferBetweenEntityHolders, platform, destPlatform, heldPrisoner);
                        job.count = 1;
                        __result = job;
                    }
                    return false;
                }
                __result = null;
                return false;
            }
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
                        var canResPawn = pawn.CanReserveAndReach(prisoner, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, forced);
                        var canResBed = pawn.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Deadly, bed.SleepingSlotsCount, 0, null, forced);
                        if (canResPawn && canResBed)
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
                            var job = JobMaker.MakeJob(prisoner.Downed ? JobDefOf.TakeWoundedPrisonerToBed : JobDefOf.EscortPrisonerToBed, prisoner, bed);
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
                else if (targetHolder is Building_HoldingPlatform destPlatform && targetHolder != currentPlatform)
                {
                    if (pawn.CanReserveAndReach(prisoner, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserveAndReach(destPlatform, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced))
                    {
                        var job = isCurrentlyOnPlatform
                            ? JobMaker.MakeJob(JobDefOf.TransferBetweenEntityHolders, currentPlatform, destPlatform, prisoner)
                            : JobMaker.MakeJob(JobDefOf.CarryToEntityHolder, destPlatform, prisoner);
                        job.count = 1;
                        __result = job;
                        return false;
                    }
                    else
                    {
                        __result = null;
                        return false;
                    }
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
