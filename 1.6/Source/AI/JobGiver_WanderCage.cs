using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobGiver_WanderCage : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.ownership?.OwnedBed is not Building_Cage cage) return null;
            var cells = cage.WanderCells;
            if (cells.Contains(pawn.Position) is false)
            {
                var moveJob = JobMaker.MakeJob(JobDefOf.GotoWander, cells.RandomElement());
                moveJob.locomotionUrgency = LocomotionUrgency.Walk;
                return moveJob;
            }

            if (Rand.Chance(0.5f) && cells.Count > 1)
            {
                var dest = cells.Where(c => c != pawn.Position).RandomElementWithFallback(pawn.Position);
                if (dest != pawn.Position)
                {
                    var moveJob = JobMaker.MakeJob(JobDefOf.GotoWander, dest);
                    moveJob.locomotionUrgency = LocomotionUrgency.Amble;
                    return moveJob;
                }
            }

            var waitJob = JobMaker.MakeJob(JobDefOf.Wait_Wander, pawn.Position);
            waitJob.expiryInterval = Rand.Range(100, 200);
            return waitJob;
        }
    }
}
