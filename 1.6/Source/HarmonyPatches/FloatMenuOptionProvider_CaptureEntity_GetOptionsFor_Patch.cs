using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(FloatMenuOptionProvider_CaptureEntity), "GetOptionsFor")]
    public static class FloatMenuOptionProvider_CaptureEntity_GetOptionsFor_Patch
    {
        public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> values, Thing clickedThing, FloatMenuContext context)
        {
            var isHuman = clickedThing is Pawn p && p.RaceProps.Humanlike && p.IsMutant is false;
            if (isHuman is false)
            {
                foreach (var v in values) yield return v;
                yield break;
            }

            if (clickedThing.TryGetComp(out CompHoldingPlatformTarget holdComp) is false || holdComp.CanBeCaptured is false || holdComp.StudiedAtHoldingPlatform is false)
            {
                yield break;
            }

            if (context.FirstSelectedPawn.CanReserveAndReach(clickedThing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true) is false)
            {
                yield return new FloatMenuOption("CannotGenericWorkCustom".Translate("CaptureLower".Translate(clickedThing)) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                yield break;
            }

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Capture".Translate(clickedThing.Label, clickedThing) + " (" + "ChooseEntityHolder".Translate() + "...)", () => StudyUtility.TargetHoldingPlatformForEntity(context.FirstSelectedPawn, clickedThing)), context.FirstSelectedPawn, clickedThing);
        }
    }
}
