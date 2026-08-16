using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(RestUtility), nameof(RestUtility.BedOwnerWillShare))]
    public static class RestUtility_BedOwnerWillShare_Patch
    {
        public static void Postfix(Building_Bed bed, Pawn sleeper, ref bool __result)
        {
            if (__result is false) return;
            var isSleeperRestrained = State.IsRestrained(sleeper);
            if (bed.OwnersForReading.Any(p => p != sleeper && (State.IsRestrained(p) || isSleeperRestrained)))
            {
                __result = false;
                return;
            }
            var baseSlots = BedUtility.GetSleepingSlotsCount(bed.def.size);
            for (var i = 0; i < baseSlots; i++)
            {
                var occupant = bed.GetCurOccupant(i);
                if (occupant != null && occupant != sleeper && (State.IsRestrained(occupant) || isSleeperRestrained))
                {
                    __result = false;
                    return;
                }
            }
        }
    }
}