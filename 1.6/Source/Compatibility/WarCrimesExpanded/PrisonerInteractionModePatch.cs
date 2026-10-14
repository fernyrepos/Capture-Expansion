using RimWorld;
using Verse;

namespace CaptureExpansion.Compatibility.WarCrimesExpanded
{
    internal static class PrisonerInteractionModePatch
    {
        public static bool Prefix(PrisonerInteractionModeDef __0)
        {
            if (!(Find.Selector.SingleSelectedThing is Building_RestraintPlatform platform))
                return true;
            Pawn pawn = platform.HeldPawn;
            if (__0 == WarCrimesExpandedCompatibility.MutilateMode
                && WarCrimesExpandedCompatibility.Eligible(pawn))
                WarCrimesExpandedCompatibility.CreateBill(pawn, true);
            return false;
        }
    }
}
