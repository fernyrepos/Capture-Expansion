using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class WorkGiver_Warden_TakeHeldPrisonerToBed : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.EntityHolder);

        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_HoldingPlatform platform || platform.HeldPawn is not { RaceProps.Humanlike: true, IsMutant: false, IsPrisonerOfColony: true } prisoner)
            {
                return false;
            }
            if (prisoner.TryGetComp<CompHoldingPlatformTarget>()?.targetHolder is not Building_Bed bed)
            {
                return false;
            }
            var validBed = bed.Spawned && bed.IsBurning() is false && (bed is Building_Cage || bed.ForPrisoners) && RestUtility.CanUseBedEver(prisoner, bed.def) && (bed.AnyUnownedSleepingSlot || bed.IsOwner(prisoner));
            var canResPlatform = pawn.CanReserveAndReach(platform, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced);
            var canResBed = pawn.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Deadly, bed.SleepingSlotsCount, 0, null, forced);
            var canResPrisoner = pawn.CanReserve(prisoner, 1, -1, null, forced);
            return validBed && canResPlatform && canResBed && canResPrisoner;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_HoldingPlatform platform || platform.HeldPawn is not { RaceProps.Humanlike: true, IsMutant: false, IsPrisonerOfColony: true } prisoner)
            {
                return null;
            }
            if (prisoner.TryGetComp<CompHoldingPlatformTarget>()?.targetHolder is not Building_Bed bed)
            {
                return null;
            }
            var job = JobMaker.MakeJob(DefsOf.CE_TakeHeldPrisonerToBed, platform, bed, prisoner);
            job.count = 1;
            return job;
        }
    }
}
