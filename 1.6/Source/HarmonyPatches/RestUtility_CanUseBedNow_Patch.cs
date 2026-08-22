using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(RestUtility), nameof(RestUtility.CanUseBedNow))]
    public static class RestUtility_CanUseBedNow_Patch
    {
        public static bool Prefix(Thing bedThing, Pawn sleeper, GuestStatus? guestStatusOverride, ref bool __result)
        {
            if (bedThing is not Building_Cage cage) return true;

            if (cage.Spawned is false || cage.Map != sleeper.MapHeld || cage.IsBurning())
            {
                __result = false;
                return false;
            }
            if (sleeper.HarmedByVacuum && cage.Position.GetVacuum(cage.Map) >= 0.5f)
            {
                __result = false;
                return false;
            }
            if (RestUtility.CanUseBedEver(sleeper, cage.def) is false)
            {
                __result = false;
                return false;
            }
            if (cage.AnyUnoccupiedSleepingSlot is false && cage.IsOwner(sleeper) is false && cage.CurOccupants.Contains(sleeper) is false)
            {
                __result = false;
                return false;
            }
            __result = (guestStatusOverride ?? sleeper.GuestStatus) == GuestStatus.Prisoner;
            return false;
        }

        public static void Postfix(Thing bedThing, Pawn sleeper, ref bool __result)
        {
            if (!__result || bedThing is not Building_Bed bed || bed is Building_Cage) return;

            var baseSlots = BedUtility.GetSleepingSlotsCount(bed.def.size);
            if (baseSlots <= 1) return;

            var isSleeperRestrained = sleeper.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed);
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
