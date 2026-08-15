using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobGiver_RestrainedLayDown : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var bed = pawn.ownership?.OwnedBed;
            if (bed != null && pawn.InBed() is false)
            {
                return JobMaker.MakeJob(JobDefOf.LayDown, bed);
            }
            return null;
        }
    }
}
