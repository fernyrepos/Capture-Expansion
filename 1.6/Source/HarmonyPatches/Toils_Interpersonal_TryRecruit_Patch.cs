using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch]
    public static class Toils_Interpersonal_TryRecruit_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Toils_Interpersonal), nameof(Toils_Interpersonal.TryRecruit));
            yield return AccessTools.Method(typeof(Toils_Interpersonal), nameof(Toils_Interpersonal.TryEnslave));
        }

        public static void Postfix(Toil __result, TargetIndex __0, MethodBase __originalMethod)
        {
            var originalInit = __result.initAction;
            var isEnslave = __originalMethod.Name == nameof(Toils_Interpersonal.TryEnslave);
            __result.initAction = () =>
            {
                var actor = __result.actor;
                if (actor.jobs.curJob.GetTarget(__0).Thing is Pawn pawn && pawn.Spawned is false && pawn.ParentHolder is Building_HoldingPlatform platform && platform.Spawned && pawn.Awake())
                {
                    if (isEnslave)
                    {
                        actor.interactions.TryInteractWith(pawn, InteractionDefOf.EnslaveAttempt);
                    }
                    else
                    {
                        var intDef = pawn.AnimalOrWildMan() ? InteractionDefOf.TameAttempt : InteractionDefOf.RecruitAttempt;
                        actor.interactions.TryInteractWith(pawn, intDef);
                    }
                    return;
                }
                originalInit();
            };
        }
    }
}
