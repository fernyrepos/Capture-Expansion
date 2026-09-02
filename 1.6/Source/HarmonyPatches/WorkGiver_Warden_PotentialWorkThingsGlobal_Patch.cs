using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden), nameof(WorkGiver_Warden.PotentialWorkThingsGlobal))]
    public static class WorkGiver_Warden_PotentialWorkThingsGlobal_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Pawn pawn)
        {
            if (__result != null)
            {
                foreach (var thing in __result)
                {
                    yield return thing;
                }
            }
            foreach (var platform in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder).OfType<Building_HoldingPlatform>())
            {
                if (platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false, IsPrisonerOfColony: true })
                {
                    yield return platform;
                }
            }
        }
    }
}
