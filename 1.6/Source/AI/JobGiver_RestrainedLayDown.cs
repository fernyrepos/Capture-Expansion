using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobGiver_RestrainedLayDown : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var bed = pawn.CurrentBed() ?? pawn.ownership?.OwnedBed ?? RestUtility.FindBedFor(pawn);
            if (bed != null)
            {
                return JobMaker.MakeJob(JobDefOf.LayDown, bed);
            }
            return null;
        }
    }
}
