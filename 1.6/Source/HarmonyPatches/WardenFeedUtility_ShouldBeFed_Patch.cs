using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WardenFeedUtility), nameof(WardenFeedUtility.ShouldBeFed))]
    public static class WardenFeedUtility_ShouldBeFed_Patch
    {
        public static void Postfix(Pawn p, ref bool __result)
        {
            if (__result || p.IsPrisonerOfColony is false || p.RaceProps.Humanlike is false || p.IsMutant)
            {
                return;
            }
            if (p.guest.CanBeBroughtFood is false)
            {
                return;
            }
            if (p.Spawned is false && p.ParentHolder is Building_HoldingPlatform)
            {
                __result = true;
                return;
            }
            if (p.ownership?.OwnedBed != null && State.IsRestrained(p))
            {
                __result = true;
            }
        }
    }
}
