using HarmonyLib;
using RimWorld;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.SleepingSlotsCount), MethodType.Getter)]
    public static class Building_Bed_SleepingSlotsCount_Patch
    {
        public static bool Prefix(Building_Bed __instance, ref int __result)
        {
            if (__instance is Building_Cage cage)
            {
                __result = cage.TotalSleepingSlots;
                return false;
            }
            return true;
        }
    }
}
