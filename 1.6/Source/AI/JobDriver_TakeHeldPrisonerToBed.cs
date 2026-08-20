using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobDriver_TakeHeldPrisonerToBed : JobDriver
    {
        protected Thing Platform => job.GetTarget(TargetIndex.A).Thing;
        protected Building_Bed DropBed => (Building_Bed)job.GetTarget(TargetIndex.B).Thing;
        protected Pawn Prisoner => (Pawn)job.GetTarget(TargetIndex.C).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Platform, job, 1, -1, null, errorOnFailed) is false || pawn.Reserve(DropBed, job, DropBed.SleepingSlotsCount, 0, null, errorOnFailed) is false) return false;
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOn(() => Prisoner == null || Prisoner.Dead);
            yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.B, TargetIndex.C);

            AddFinishAction(jobCondition =>
            {
                if (jobCondition != JobCondition.Ongoing && pawn.carryTracker.CarriedThing != null)
                {
                    pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out _);
                }
            });

            var goToPlatform = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnDespawnedNullOrForbidden(TargetIndex.B);
            yield return Toils_Jump.JumpIf(goToPlatform, () => pawn.IsCarryingPawn(Prisoner) is false);

            var takePawn = ToilMaker.MakeToil("TakePawnFromPlatform");
            takePawn.initAction = () =>
            {
                if (Platform is Building_HoldingPlatform holdingPlatform && holdingPlatform.HeldPawn == Prisoner)
                {
                    holdingPlatform.HeldPawn.GetComp<CompHoldingPlatformTarget>()?.Notify_ReleasedFromPlatform();
                    pawn.carryTracker.innerContainer.TryAddOrTransfer(Prisoner);
                }
            };
            takePawn.defaultCompleteMode = ToilCompleteMode.Instant;

            var goToBed = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOn(() => pawn.IsCarryingPawn(Prisoner) is false);
            goToBed.FailOnBedNoLongerUsable(TargetIndex.B, TargetIndex.C);

            yield return goToPlatform;
            yield return takePawn;
            yield return goToBed;
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield return Toils_Bed.TuckIntoBed(DropBed, pawn, Prisoner);
        }
    }
}
