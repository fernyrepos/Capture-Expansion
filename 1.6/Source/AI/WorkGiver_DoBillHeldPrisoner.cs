using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    public class WorkGiver_DoBillHeldPrisoner : WorkGiver_DoBill
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (var platform in pawn.Map.listerThings.AllThings.OfType<Building_HoldingPlatform>())
            {
                var held = platform.HeldPawn;
                if (held != null && held.RaceProps.Humanlike && held.IsMutant is false && held.BillStack != null && held.BillStack.AnyShouldDoNow)
                {
                    yield return held;
                }
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return PotentialWorkThingsGlobal(pawn).Any() is false;
        }
    }
}
