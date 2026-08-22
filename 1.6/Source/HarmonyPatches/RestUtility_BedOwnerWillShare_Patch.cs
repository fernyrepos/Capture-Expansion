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
            var baseSlots = BedUtility.GetSleepingSlotsCount(bed.def.size);
            if (baseSlots <= 1) return;

            var isSleeperRestrained = sleeper.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed);
            var owners = bed.OwnersForReading;
            for (int i = 0; i < owners.Count; i++)
            {
                var p = owners[i];
                if (p != sleeper && (p.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed) || isSleeperRestrained))
                {
                    __result = false;
                    return;
                }
            }
            for (var i = 0; i < baseSlots; i++)
            {
                var occupant = bed.GetCurOccupant(i);
                if (occupant != null && occupant != sleeper && (occupant.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed) || isSleeperRestrained))
                {
                    __result = false;
                    return;
                }
            }
        }
    }
}