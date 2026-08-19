using RimWorld;
using Verse;

namespace CaptureExpansion
{
    public class ThoughtWorker_BrutallyRestrained : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.IsPrisoner is false && p.IsSlave is false)
            {
                return ThoughtState.Inactive;
            }
            if (p.RaceProps.Humanlike is false || p.IsMutant)
            {
                return ThoughtState.Inactive;
            }
            if (State.IsRestrained(p) || State.IsCaged(p))
            {
                return ThoughtState.ActiveDefault;
            }
            return ThoughtState.Inactive;
        }
    }
}
