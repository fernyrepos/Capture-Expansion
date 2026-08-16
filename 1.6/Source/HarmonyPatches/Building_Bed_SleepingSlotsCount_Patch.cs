using System.Linq;
using HarmonyLib;
using RimWorld;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.SleepingSlotsCount), MethodType.Getter)]
    public static class Building_Bed_SleepingSlotsCount_Patch
    {
        public static void Postfix(Building_Bed __instance, ref int __result)
        {
            if (__instance is Building_Cage cage)
            {
                __result = cage.TotalSleepingSlots;
            }
            else if (__instance.OwnersForReading.Any(State.IsRestrained))
            {
                __result = 1;
            }
            else
            {
                var baseSlots = BedUtility.GetSleepingSlotsCount(__instance.def.size);
                for (var i = 0; i < baseSlots; i++)
                {
                    var occupant = __instance.GetCurOccupant(i);
                    if (occupant != null && State.IsRestrained(occupant))
                    {
                        __result = 1;
                        break;
                    }
                }
            }
        }
    }
}
