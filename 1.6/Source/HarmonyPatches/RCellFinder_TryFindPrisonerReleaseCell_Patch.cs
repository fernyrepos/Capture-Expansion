using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(RCellFinder), nameof(RCellFinder.TryFindPrisonerReleaseCell))]
    public static class RCellFinder_TryFindPrisonerReleaseCell_Patch
    {
        public static bool Prefix(Pawn prisoner, Pawn warden, ref IntVec3 result, ref bool __result)
        {
            if (prisoner.Spawned is false && prisoner.ParentHolder is Building_HoldingPlatform platform)
            {
                if (platform.Map != warden.Map)
                {
                    result = IntVec3.Invalid;
                    __result = false;
                    return false;
                }
                var region = platform.GetRegion();
                if (region == null)
                {
                    result = IntVec3.Invalid;
                    __result = false;
                    return false;
                }
                var traverseParms = TraverseParms.For(warden);
                var needMapEdge = prisoner.Faction != warden.Faction;
                var foundResult = IntVec3.Invalid;
                RegionTraverser.BreadthFirstTraverse(region, (from, r) => r.Allows(traverseParms, isDestination: false), r =>
                {
                    if (needMapEdge)
                    {
                        if (r.District.TouchesMapEdge is false)
                        {
                            return false;
                        }
                    }
                    else if (r.Room.IsPrisonCell)
                    {
                        return false;
                    }
                    foundResult = r.RandomCell;
                    return true;
                }, 999);
                result = foundResult;
                __result = foundResult.IsValid;
                return false;
            }
            return true;
        }
    }
}
