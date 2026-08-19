using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(StudyUtility), nameof(StudyUtility.TargetHoldingPlatformForEntity))]
    public static class StudyUtility_TargetHoldingPlatformForEntity_Patch
    {
        public static bool Prefix(Pawn carrier, Thing entity, bool transferBetweenPlatforms, Thing sourcePlatform)
        {
            if (entity is not Pawn pawn || pawn.RaceProps.Humanlike is false || pawn.IsMutant) return true;

            Find.Targeter.BeginTargeting(TargetingParameters.ForBuilding(), target =>
            {
                if (target.Thing is Building_Bed bed)
                {
                    if (carrier != null)
                    {
                        var isCurrentlyOnPlatform = pawn.ParentHolder is Building_HoldingPlatform;
                        var src = sourcePlatform ?? pawn.ParentHolder as Thing;
                        var job = isCurrentlyOnPlatform && src != null
                            ? JobMaker.MakeJob(DefsOf.CE_TakeHeldPrisonerToBed, src, bed, pawn)
                            : JobMaker.MakeJob(pawn.Downed ? JobDefOf.TakeWoundedPrisonerToBed : JobDefOf.EscortPrisonerToBed, pawn, bed);
                        job.count = 1;
                        carrier.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }
                    else
                    {
                        if (pawn.ParentHolder is Building_HoldingPlatform)
                        {
                            pawn.TryGetComp<CompHoldingPlatformTarget>().targetHolder = bed;
                        }
                        else
                        {
                            pawn.ownership.ClaimBedIfNonMedical(bed);
                        }
                    }
                }
                else if (target.Thing is Building_HoldingPlatform platform)
                {
                    pawn.TryGetComp<CompHoldingPlatformTarget>().targetHolder = platform;
                    if (carrier != null)
                    {
                        var isCurrentlyOnPlatform = pawn.ParentHolder is Building_HoldingPlatform;
                        var src = sourcePlatform ?? pawn.ParentHolder as Thing;
                        var job = isCurrentlyOnPlatform && src != null
                            ? JobMaker.MakeJob(JobDefOf.TransferBetweenEntityHolders, src, platform, pawn)
                            : JobMaker.MakeJob(JobDefOf.CarryToEntityHolder, platform, pawn);
                        job.count = 1;
                        carrier.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }
                }
            }, target =>
            {
                if (IsValid(target, carrier, pawn)) GenDraw.DrawTargetHighlight(target);
            }, target => IsValid(target, carrier, pawn), onUpdateAction: _ =>
            {
                foreach (var b in pawn.MapHeld.listerBuildings.allBuildingsColonist)
                {
                    if (IsValid(b, carrier, pawn)) GenDraw.DrawArrowPointingAt(b.DrawPos);
                }
            });
            return false;
        }

        private static bool IsValid(LocalTargetInfo target, Pawn carrier, Pawn victim)
        {
            if (target.Thing is Building_HoldingPlatform platform)
                return platform.TryGetComp<CompEntityHolder>() is { Available: true } && (carrier == null || carrier.CanReserveAndReach(platform, PathEndMode.Touch, Danger.Some));
            if (target.Thing is Building_Bed bed)
                return bed.IsBurning() is false && (bed is Building_Cage || bed.ForPrisoners) && RestUtility.CanUseBedEver(victim, bed.def) && (bed.AnyUnownedSleepingSlot || bed.IsOwner(victim)) && (carrier == null || carrier.CanReserveAndReach(bed, PathEndMode.Touch, Danger.Some));
            return false;
        }
    }
}
