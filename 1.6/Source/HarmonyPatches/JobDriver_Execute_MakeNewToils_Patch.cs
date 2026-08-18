using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(JobDriver_Execute), nameof(JobDriver_Execute.MakeNewToils))]
    public static class JobDriver_Execute_MakeNewToils_Patch
    {
        public static void Postfix(JobDriver_Execute __instance, ref IEnumerable<Toil> __result)
        {
            if (__instance.job.targetA.Thing is not Pawn prisoner || prisoner.ParentHolder is not Building_HoldingPlatform platform)
            {
                return;
            }
            var toils = __result.ToList();
            if (toils.Count < 2)
            {
                return;
            }
            toils.Insert(1, MakeEjectToil(platform, prisoner));
            __result = toils;
        }

        public static Toil MakeEjectToil(Building_HoldingPlatform platform, Pawn prisoner)
        {
            var toil = ToilMaker.MakeToil("EjectHeldPrisoner");
            toil.initAction = () =>
            {
                if (platform.HeldPawn == prisoner)
                {
                    platform.EjectContents();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            return toil;
        }
    }
}
