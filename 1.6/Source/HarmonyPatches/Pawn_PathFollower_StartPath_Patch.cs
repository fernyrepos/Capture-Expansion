using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
    public static class Pawn_PathFollower_StartPath_Patch
    {
        public static void Prefix(ref LocalTargetInfo dest, ref PathEndMode peMode)
        {
            if (dest.HasThing && dest.Thing is Pawn p && p.Spawned is false && p.ParentHolder is Building_HoldingPlatform platform)
            {
                dest = platform;
                peMode = PathEndMode.ClosestTouch;
            }
        }
    }
}
