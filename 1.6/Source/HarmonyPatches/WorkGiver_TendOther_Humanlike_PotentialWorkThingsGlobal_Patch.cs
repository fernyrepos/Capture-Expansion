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
        public static void Postfix(Pawn pawn, WorkGiver_Tend __instance, ref IEnumerable<Thing> __result)
        {
            if (__instance is not WorkGiver_TendOther_Humanlike)
            {
                return;
            }
            var list = __result.ToList();
            foreach (var platform in pawn.Map.listerThings.AllThings.OfType<Building_HoldingPlatform>())
            {
                var held = platform.HeldPawn;
                if (held != null && held.RaceProps.Humanlike && held.IsMutant is false && HealthAIUtility.ShouldBeTendedNowByPlayer(held))
                {
                    list.Add(held);
                }
            }
            __result = list;
        }
    }
}
