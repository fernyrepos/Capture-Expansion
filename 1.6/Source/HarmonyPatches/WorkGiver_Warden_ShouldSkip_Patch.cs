using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden), nameof(WorkGiver_Warden.ShouldSkip))]
    public static class WorkGiver_Warden_ShouldSkip_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result is false)
            {
                return;
            }
            foreach (var platform in pawn.Map.listerThings.AllThings.OfType<Building_HoldingPlatform>())
            {
                if (platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false, IsPrisonerOfColony: true })
                {
                    __result = false;
                    return;
                }
            }
        }
    }
}
