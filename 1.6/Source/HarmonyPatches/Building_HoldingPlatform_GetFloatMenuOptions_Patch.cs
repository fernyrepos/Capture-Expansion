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
        }
    }
}
