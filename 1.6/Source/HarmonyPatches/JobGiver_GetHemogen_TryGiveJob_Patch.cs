using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(JobGiver_GetHemogen), nameof(JobGiver_GetHemogen.TryGiveJob))]
    public static class JobGiver_GetHemogen_TryGiveJob_Patch
    {
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            if (__result != null || ModsConfig.BiotechActive is false)
            {
                return;
            }
            var gene = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (gene == null || pawn.IsBloodfeeder() is false || gene.ShouldConsumeHemogenNow() is false)
            {
                return;
            }
            foreach (var platform in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder).OfType<Building_HoldingPlatform>())
            {
                var held = platform.HeldPawn;
                if (held == null || held.RaceProps.Humanlike is false || held.IsMutant || held.WouldDieFromAdditionalBloodLoss(0.4499f))
                {
                    continue;
                }
                if (held.IsPrisonerOfColony is false || held.guest.PrisonerIsSecure is false || held.guest.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed) || held.InAggroMentalState || platform.IsForbidden(pawn) || pawn.CanReserveAndReach(held, PathEndMode.ClosestTouch, pawn.NormalMaxDanger()) is false)
                {
                    continue;
                }
                __result = JobMaker.MakeJob(JobDefOf.PrisonerBloodfeed, held);
                return;
            }
        }
    }
}
