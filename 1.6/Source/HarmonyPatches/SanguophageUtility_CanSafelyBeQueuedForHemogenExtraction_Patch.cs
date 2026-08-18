using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(SanguophageUtility), nameof(SanguophageUtility.CanSafelyBeQueuedForHemogenExtraction))]
    public static class SanguophageUtility_CanSafelyBeQueuedForHemogenExtraction_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result || ModsConfig.BiotechActive is false)
            {
                return;
            }
            if (pawn.Spawned || pawn.RaceProps.Humanlike is false || pawn.IsMutant || State.IsRestrained(pawn) is false)
            {
                return;
            }
            if (pawn.BillStack.Bills.Any(x => x.recipe == RecipeDefOf.ExtractHemogenPack))
            {
                return;
            }
            if (SanguophageUtility.PawnConsciousEnoughForExtraction(pawn) is false || RecipeDefOf.ExtractHemogenPack.Worker.AvailableOnNow(pawn) is false)
            {
                return;
            }
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
            {
                return;
            }
            __result = true;
        }
    }
}
