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
                var isMasochist = p.story?.traits?.HasTrait(DefsOf.Masochist) ?? false;
                return ThoughtState.ActiveAtStage(isMasochist ? 1 : 0);
            }
            return ThoughtState.Inactive;
        }
    }
}
