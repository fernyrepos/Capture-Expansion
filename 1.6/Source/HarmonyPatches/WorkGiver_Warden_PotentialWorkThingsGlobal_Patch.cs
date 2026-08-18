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
        public static void Postfix(Pawn pawn, ref IEnumerable<Thing> __result)
        {
            var list = __result.ToList();
            foreach (var platform in pawn.Map.listerThings.AllThings.OfType<Building_HoldingPlatform>())
            {
                var held = platform.HeldPawn;
                if (held != null && held.RaceProps.Humanlike && held.IsMutant is false && held.IsPrisonerOfColony)
                {
                    list.Add(held);
                }
            }
            __result = list;
        }
    }
}
