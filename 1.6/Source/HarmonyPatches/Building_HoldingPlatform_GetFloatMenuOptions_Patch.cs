using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_HoldingPlatform), "GetFloatMenuOptions")]
    public static class Building_HoldingPlatform_GetFloatMenuOptions_Patch
    {
        public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> values, Building_HoldingPlatform __instance, Pawn selPawn)
        {
            foreach (var v in values) yield return v;
            var held = __instance.HeldPawn;
            if (held != null && held.RaceProps.Humanlike && held.AnythingToStrip())
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Strip".Translate(held.LabelCap, held), () =>
                {
                    __instance.SetForbidden(false, warnOnFail: false);
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(DefsOf.CE_StripHeldEntity, __instance), JobTag.Misc);
                    StrippableUtility.CheckSendStrippingImpactsGoodwillMessage(held);
                }), selPawn, __instance);
            }
            if (held == null || held.RaceProps.Humanlike is false || held.IsMutant || held.IsPrisonerOfColony is false)
            {
                yield break;
            }
            if (ModsConfig.BiotechActive && held.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Bloodfeed) is false)
            {
                yield break;
            }
            if (selPawn.IsBloodfeeder() is false || selPawn.genes.GetFirstGeneOfType<Gene_Hemogen>() == null || held.InAggroMentalState || held.WouldDieFromAdditionalBloodLoss(0.4499f))
            {
                yield break;
            }
            if (selPawn.CanReach(__instance, PathEndMode.ClosestTouch, Danger.Deadly) is false)
            {
                yield return new FloatMenuOption("CannotBloodfeedOn".Translate(held.Named("PAWN")) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                yield break;
            }
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("BloodfeedOn".Translate(held.Named("PAWN")), () =>
            {
                __instance.SetForbidden(false, warnOnFail: false);
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.PrisonerBloodfeed, held), JobTag.Misc);
            }), selPawn, __instance);
        }
    }
}
