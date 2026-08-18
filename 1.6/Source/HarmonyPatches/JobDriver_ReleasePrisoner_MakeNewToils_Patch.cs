using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(JobDriver_ReleasePrisoner), nameof(JobDriver_ReleasePrisoner.MakeNewToils))]
    public static class JobDriver_ReleasePrisoner_MakeNewToils_Patch
    {
        public static void Postfix(JobDriver_ReleasePrisoner __instance, ref IEnumerable<Toil> __result)
        {
            if (__instance.job.targetA.Thing is not Pawn prisoner || prisoner.ParentHolder is not Building_HoldingPlatform platform)
            {
                return;
            }
            var toils = __result.ToList();
            __instance.globalFailConditions.Clear();
            __instance.AddFailCondition(() => __instance.job.targetA.Thing.DestroyedOrNull() || __instance.job.targetB.ToTargetInfo(__instance.Map).IsBurning() || ((Pawn)__instance.job.targetA.Thing).guest.IsInteractionDisabled(PrisonerInteractionModeDefOf.Release) || ((Pawn)__instance.job.targetA.Thing).InAggroMentalState);
            toils.Insert(1, JobDriver_Execute_MakeNewToils_Patch.MakeEjectToil(platform, prisoner));
            __result = toils;
        }
    }
}
