using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ReachabilityImmediate), nameof(ReachabilityImmediate.CanReachImmediate), new Type[] {
        typeof(IntVec3),
        typeof(LocalTargetInfo),
        typeof(Map),
        typeof(PathEndMode),
        typeof(Pawn)
    })]
    public static class ReachabilityImmediate_CanReachImmediate_Patch
    {
        public static bool Prefix(IntVec3 start, LocalTargetInfo target, ref bool __result)
        {
            if (target.HasThing && target.Thing is Pawn { Spawned: false, ParentHolder: Building_HoldingPlatform platform })
            {
                foreach (var cell in platform.OccupiedRect().ExpandedBy(1).EdgeCellsNoCorners)
                {
                    if (start == cell)
                    {
                        __result = true;
                        return false;
                    }
                }
                __result = false;
                return false;
            }
            return true;
        }
    }
}
