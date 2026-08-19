using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), new Type[] {
        typeof(IntVec3),
        typeof(LocalTargetInfo),
        typeof(PathEndMode),
        typeof(TraverseParms)
    })]
    public static class Reachability_CanReach_Patch
    {
        public static void Prefix(ref LocalTargetInfo dest, ref PathEndMode peMode)
        {
            if (dest.HasThing && dest.Thing is Pawn { Spawned: false, ParentHolder: Building_HoldingPlatform platform })
            {
                dest = platform;
                if (peMode == PathEndMode.OnCell)
                {
                    peMode = PathEndMode.ClosestTouch;
                }
            }
        }
    }
}
