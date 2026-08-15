using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.FindPreferredInteractionCell))]
    public static class Building_Bed_FindPreferredInteractionCell_Patch
    {
        public static bool Prefix(Building_Bed __instance, ref IntVec3? __result)
        {
            if (__instance is Building_Cage cage)
            {
                var rect = cage.OccupiedRect();
                var map = cage.Map;
                foreach (var cell in GenAdjFast.AdjacentCells8Way(cage.Position, cage.Rotation, cage.def.size))
                {
                    if (cell.InBounds(map) && cell.Standable(map) && cell.GetDoor(map) == null && ReachabilityImmediate.CanReachImmediate(cell, rect, map, PathEndMode.Touch, null))
                    {
                        __result = cell;
                        return false;
                    }
                }
                foreach (var cell in GenAdjFast.AdjacentCells8Way(cage.Position, cage.Rotation, cage.def.size))
                {
                    if (cell.InBounds(map) && cell.Standable(map) && ReachabilityImmediate.CanReachImmediate(cell, rect, map, PathEndMode.Touch, null))
                    {
                        __result = cell;
                        return false;
                    }
                }
                __result = cage.Position;
                return false;
            }
            return true;
        }
    }
}
