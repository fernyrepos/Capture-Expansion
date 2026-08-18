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
    }
}
