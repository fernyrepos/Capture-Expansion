using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ThinkNode_JobGiver), nameof(ThinkNode_JobGiver.TryIssueJobPackage))]
    public static class ThinkNode_JobGiver_TryIssueJobPackage_Patch
    {
        public static void Postfix(Pawn pawn, ref ThinkResult __result)
        {
            var job = __result.Job;
            if (job == null || pawn.IsPrisoner is false || PrisonBreakUtility.IsPrisonBreaking(pawn) || pawn.ownership?.OwnedBed is not Building_Cage cage) return;
            var rect = cage.OccupiedRect();
            if (TargetOutside(job.targetA, rect) || TargetOutside(job.targetB, rect) || TargetOutside(job.targetC, rect))
            {
                __result = ThinkResult.NoJob;
                return;
            }
            if (QueueOutside(job.targetQueueA, rect) || QueueOutside(job.targetQueueB, rect))
            {
                __result = ThinkResult.NoJob;
            }
        }

        private static bool TargetOutside(LocalTargetInfo target, CellRect rect)
        {
            if (target.IsValid is false || target.HasThing && target.Thing.Spawned is false) return false;
            return rect.Contains(target.Cell) is false;
        }

        private static bool QueueOutside(List<LocalTargetInfo> queue, CellRect rect)
        {
            if (queue.NullOrEmpty()) return false;
            foreach (var item in queue)
            {
                if (TargetOutside(item, rect)) return true;
            }
            return false;
        }
    }
}
