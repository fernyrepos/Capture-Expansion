using RimWorld;
using Verse;

namespace CaptureExpansion
{
    public class Hediff_RestrainedToBed : Hediff
    {
        public override void Tick()
        {
            base.Tick();
            if (pawn.IsHashIntervalTick(60) && pawn.InBed() is false)
            {
                pawn.health.RemoveHediff(this);
            }
        }
    }
}
