using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch(typeof(JobDriver_DeliverPawnToAltar), "MakeNewToils")]
    internal static class RitualPrisonerDeliveryJobPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static void Postfix(JobDriver_DeliverPawnToAltar __instance, ref IEnumerable<Toil> __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(__instance.job.GetTarget(TargetIndex.C).Thing);
            if (platform == null || !(__instance.pawn.GetLord()?.LordJob is LordJob_Ritual ritual)
                || !RitualExecutionUtility.Supported(ritual.Ritual) || ritual.selectedTarget.Thing != platform)
                return;
            Pawn victim = __instance.job.GetTarget(TargetIndex.A).Thing as Pawn;
            if (victim != null && ritual.PawnWithRole("prisoner") == victim)
                __result = Deliver(__instance, platform, ritual, victim);
        }

        private static IEnumerable<Toil> Deliver(JobDriver_DeliverPawnToAltar driver,
            Building_RestraintPlatform platform, LordJob_Ritual ritual, Pawn victim)
        {
            driver.FailOnDestroyedOrNull(TargetIndex.A);
            driver.FailOnDespawnedNullOrForbidden(TargetIndex.C);
            driver.FailOn(() => driver.pawn.GetLord()?.LordJob != ritual
                || !RitualExecutionUtility.ActiveTargetAllowed(platform, ritual));
            driver.AddFinishAction(condition =>
            {
                if (driver.pawn.IsCarryingPawn(victim))
                    driver.pawn.carryTracker.TryDropCarriedThing(driver.pawn.Position, ThingPlaceMode.Near, out _);
            });
            if (platform.HeldPawn == victim)
                yield break;
            CompEntityHolder holder = platform.TryGetComp<CompEntityHolder>();
            driver.FailOn(() => holder == null || (!holder.Available && holder.HeldPawn != victim));
            // Keep saved chaining toil indices unchanged.
            Toil checkCarriedPrisoner = ToilMaker.MakeToil("CheckCarriedPrisoner");
            checkCarriedPrisoner.FailOn(() => !driver.pawn.IsCarryingPawn(victim));
            yield return Toils_Jump.JumpIf(checkCarriedPrisoner, () => driver.pawn.IsCarryingPawn(victim));
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return checkCarriedPrisoner;
            foreach (Toil toil in JobDriver_CarryToEntityHolder.ChainTakeeToPlatformToils(
                driver.pawn, victim, holder, TargetIndex.C))
                yield return toil;
        }
    }
}
