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
            foreach (var platform in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder).OfType<Building_HoldingPlatform>())
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
            foreach (var platform in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder).OfType<Building_HoldingPlatform>())
            {
                if (platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false, BillStack: { } bills } && bills.AnyShouldDoNow)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
