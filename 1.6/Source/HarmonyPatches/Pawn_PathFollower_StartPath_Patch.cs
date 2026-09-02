using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
    public static class Pawn_PathFollower_StartPath_Patch
    {
        public static void Prefix(Pawn ___pawn, ref LocalTargetInfo dest, ref PathEndMode peMode)
        {
            if (dest.HasThing && dest.Thing is Pawn { Spawned: false, ParentHolder: Building_HoldingPlatform platform })
            {
                var best = IntVec3.Invalid;
                var bestDist = float.MaxValue;
                foreach (var cell in platform.OccupiedRect().ExpandedBy(1).EdgeCellsNoCorners)
                {
                    if (cell.InBounds(___pawn.Map) is false || cell.Standable(___pawn.Map) is false)
                    {
                        continue;
                    }
                    var dist = (cell - ___pawn.Position).LengthHorizontalSquared;
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        best = cell;
                    }
                }
                if (best.IsValid)
                {
                    dest = best;
                    peMode = PathEndMode.OnCell;
                }
                else
                {
                    dest = platform;
                    peMode = PathEndMode.Touch;
                }
            }
        }
    }
}
