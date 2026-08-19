using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Tend), nameof(WorkGiver_Tend.PotentialWorkThingsGlobal))]
    public static class WorkGiver_TendOther_Humanlike_PotentialWorkThingsGlobal_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Pawn pawn, WorkGiver_Tend __instance)
        {
            if (__result != null)
            {
                foreach (var thing in __result)
                {
                    yield return thing;
                }
            }
            if (__instance is not WorkGiver_TendOther_Humanlike)
            {
                yield break;
            }
            foreach (var platform in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder).OfType<Building_HoldingPlatform>())
            {
                if (platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false } held && HealthAIUtility.ShouldBeTendedNowByPlayer(held))
                {
                    yield return held;
                }
            }
        }
    }
}
