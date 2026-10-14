using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch(typeof(JobDriver_Sacrifice), "MakeNewToils")]
    internal static class RitualExecutionJobPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static void Postfix(JobDriver_Sacrifice __instance, ref IEnumerable<Toil> __result)
        {
            if (!(__instance.pawn.GetLord()?.LordJob is LordJob_Ritual ritual)
                || !RitualExecutionUtility.Supported(ritual.Ritual))
                return;
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(ritual.selectedTarget.Thing);
            Pawn victim = __instance.job.GetTarget(TargetIndex.A).Thing as Pawn;
            if (platform != null && victim != null && ritual.PawnWithRole("prisoner") == victim)
                __result = Execute(__instance, platform, ritual, victim);
        }

        private static IEnumerable<Toil> Execute(JobDriver_Sacrifice driver,
            Building_RestraintPlatform platform, LordJob_Ritual ritual, Pawn victim)
        {
            driver.job.SetTarget(TargetIndex.C, platform);
            driver.FailOnDestroyedOrNull(TargetIndex.A);
            driver.FailOnDestroyedOrNull(TargetIndex.C);
            driver.FailOn(() => driver.pawn.GetLord()?.LordJob != ritual
                || !RitualExecutionUtility.ActiveTargetAllowed(platform, ritual) || victim.Dead);
            yield return Toils_Reserve.Reserve(TargetIndex.C);
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.C, 35, useProgressBar: true);
            yield return Toils_General.Do(() =>
            {
                if (!RitualExecutionUtility.ActiveTargetAllowed(platform, ritual) || victim.Dead)
                {
                    driver.EndJobWith(JobCondition.Incompletable);
                    return;
                }
                ritual.pawnsDeathIgnored.Add(victim);
                victim.health.killedByRitual = true;
                if (platform.TryGetComp<CompGuillotine>() != null)
                {
                    CompGuillotine.Behead(victim);
                    DefsOf.CE_GuillotineActivate.PlayOneShot(SoundInfo.InMap(platform));
                    GenPlace.TryPlaceThing(ThingMaker.MakeThing(DefsOf.Skull), platform.Position,
                        platform.Map, ThingPlaceMode.Near);
                }
                else
                {
                    CompElectricChair.Electrocute(platform, victim);
                    DefsOf.CE_ElectricChairActivate.PlayOneShot(SoundInfo.InMap(platform));
                }
                ThoughtUtility.GiveThoughtsForPawnExecuted(victim, driver.pawn, PawnExecutionKind.GenericBrutal);
                TaleRecorder.RecordTale(TaleDefOf.ExecutedPrisoner, driver.pawn, victim);
            });
        }
    }
}
