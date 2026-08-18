using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Toils_Interpersonal), nameof(Toils_Interpersonal.GotoInteractablePosition))]
    public static class Toils_Interpersonal_GotoInteractablePosition_Patch
    {
        public static void Postfix(Toil __result, TargetIndex target)
        {
            var original = __result.tickIntervalAction;
            if (original == null)
            {
                return;
            }
            __result.tickIntervalAction = delta =>
            {
                var actor = __result.actor;
                var pawn = (Pawn)(Thing)actor.CurJob.GetTarget(target);
                if (pawn.Spawned is false && pawn.ParentHolder is Building_HoldingPlatform platform)
                {
                    var map = actor.Map;
                    if (SocialInteractionUtility.IsGoodPositionForInteraction(actor, pawn) && actor.Position.InHorDistOf(platform.Position, Mathf.CeilToInt(3f)) && (actor.pather.Moving is false || actor.pather.nextCell.GetDoor(map) == null))
                    {
                        actor.pather.StopDead();
                        actor.jobs.curDriver.ReadyForNextToil();
                    }
                    else if (actor.pather.Moving is false)
                    {
                        var intVec = SocialInteractionUtility.BestInteractableCell(actor, pawn);
                        if (intVec.IsValid)
                        {
                            actor.pather.StartPath(intVec, PathEndMode.OnCell);
                        }
                        else
                        {
                            actor.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                        }
                    }
                    return;
                }
                original(delta);
            };
        }
    }
}
