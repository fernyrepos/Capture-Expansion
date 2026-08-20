using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_TransferEntity), nameof(WorkGiver_TransferEntity.JobOnThing))]
    public static class WorkGiver_TransferEntity_JobOnThing_Patch
    {
        public static bool Prefix(Thing t, ref Job __result)
        {
            if (t is Building_HoldingPlatform { HeldPawn: var held } && held != null && held.TryGetComp<CompHoldingPlatformTarget>()?.targetHolder is Building_Bed bed && held.RaceProps.Humanlike && held.IsMutant is false)
            {
                var job = JobMaker.MakeJob(DefsOf.CE_TakeHeldPrisonerToBed, t, bed, held);
                job.count = 1;
                __result = job;
                return false;
            }
            return true;
        }
    }
}
