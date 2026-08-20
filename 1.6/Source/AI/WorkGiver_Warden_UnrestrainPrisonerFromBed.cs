using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class WorkGiver_Warden_UnrestrainPrisonerFromBed : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Pawn prisoner || ShouldTakeCareOfPrisoner(pawn, t, forced) is false)
            {
                return null;
            }
            if (prisoner.guest.IsInteractionEnabled(DefsOf.CE_RestrainToBed) || prisoner.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed) is false)
            {
                return null;
            }
            return JobMaker.MakeJob(DefsOf.CE_UnrestrainPrisonerFromBed, prisoner);
        }
    }
}
