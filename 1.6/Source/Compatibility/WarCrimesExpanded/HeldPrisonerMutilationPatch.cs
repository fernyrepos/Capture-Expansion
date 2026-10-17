using RimWorld;
using Verse;

namespace CaptureExpansion.Compatibility.WarCrimesExpanded
{
    internal static class HeldPrisonerMutilationPatch
    {
        public static void Postfix(Pawn __1, GuestStatus __2, PrisonerInteractionModeDef __3)
        {
            Pawn pawn = __1;
            if (pawn == null || pawn.Spawned || __2 != GuestStatus.Prisoner
                || __3 != WarCrimesExpandedCompatibility.MutilateMode || !pawn.IsHashIntervalTick(15000))
                return;
            if (WarCrimesExpandedCompatibility.Eligible(pawn)
                && (double)pawn.health.hediffSet.BleedRateTotal <= 0.1)
                WarCrimesExpandedCompatibility.CreateBill(pawn, false);
        }
    }
}
