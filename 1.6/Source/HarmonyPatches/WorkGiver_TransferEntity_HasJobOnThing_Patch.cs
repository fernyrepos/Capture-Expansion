using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_TransferEntity), "HasJobOnThing")]
    public static class WorkGiver_TransferEntity_HasJobOnThing_Patch
    {
        public static void Postfix(Pawn pawn, Thing t, bool forced, ref bool __result)
        {
            if (t is not Building_HoldingPlatform { HeldPawn: var held } || held == null) return;
            if (held.TryGetComp<CompHoldingPlatformTarget>()?.targetHolder is Building_Bed bed && held.RaceProps.Humanlike && held.IsMutant is false)
            {
                var canReservePlatform = pawn.CanReserve(t, 1, -1, null, forced);
                var canReserveHeld = pawn.CanReserve(held, 1, -1, null, forced);
                var canReserveBed = pawn.CanReserve(bed, bed.SleepingSlotsCount, 0, null, forced);
                var validBed = bed.Spawned && bed.IsBurning() is false && (bed is Building_Cage || bed.ForPrisoners) && RestUtility.CanUseBedEver(held, bed.def) && (bed.AnyUnownedSleepingSlot || bed.IsOwner(held));
                __result = canReservePlatform && canReserveHeld && canReserveBed && validBed;
            }
        }
    }
}
