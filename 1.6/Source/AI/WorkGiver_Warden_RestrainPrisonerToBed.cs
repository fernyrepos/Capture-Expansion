using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class WorkGiver_Warden_RestrainPrisonerToBed : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Pawn prisoner || ShouldTakeCareOfPrisoner(pawn, t, forced) is false)
            {
                return null;
            }
            if (prisoner.IsCaged() || prisoner.guest.IsInteractionEnabled(DefsOf.CE_RestrainToBed) is false || prisoner.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed))
            {
                return null;
            }
            var bed = prisoner.CurrentBed() ?? prisoner.ownership?.OwnedBed ?? RestUtility.FindBedFor(prisoner, pawn, checkSocialProperness: false, ignoreOtherReservations: false, GuestStatus.Prisoner);
            if (bed == null || bed is Building_Cage || bed.Spawned is false || bed.IsBurning() || RestUtility.CanUseBedEver(prisoner, bed.def) is false || pawn.CanReserve(bed, bed.SleepingSlotsCount, 0, null, forced) is false)
            {
                return null;
            }
            var job = JobMaker.MakeJob(DefsOf.CE_RestrainPrisonerToBed, prisoner, bed);
            job.count = 1;
            return job;
        }
    }
}
