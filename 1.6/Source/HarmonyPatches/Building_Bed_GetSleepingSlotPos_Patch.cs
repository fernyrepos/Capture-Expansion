using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.GetSleepingSlotPos))]
    public static class Building_Bed_GetSleepingSlotPos_Patch
    {
        public static bool Prefix(Building_Bed __instance, int index, ref IntVec3 __result)
        {
            if (__instance is not Building_Cage cage)
                return true;
            var cells = cage.WanderCells;
            __result = cells[index % cells.Count];
            return false;
        }
    }
}
