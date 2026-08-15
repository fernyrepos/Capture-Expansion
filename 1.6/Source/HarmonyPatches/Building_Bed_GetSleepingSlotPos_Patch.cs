using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.GetSleepingSlotPos))]
    public static class Building_Bed_GetSleepingSlotPos_Patch
    {
        public static void Postfix(Building_Bed __instance, int index, ref IntVec3 __result)
        {
            if (__instance is Building_Cage cage)
            {
                var cells = cage.WanderCells().ToList();
                if (cells.Count > 0)
                {
                    __result = cells[Math.Min(index, cells.Count - 1)];
                }
            }
        }
    }
}
