using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobDriver_RestrainPrisonerToBed : JobDriver
    {
        protected Pawn Prisoner => (Pawn)job.GetTarget(TargetIndex.A).Thing;
        protected Building_Bed Bed => (Building_Bed)job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Prisoner.ClearAllReservations();
            if (pawn.Reserve(Prisoner, job, 1, -1, null, errorOnFailed) is false) return false;
            return pawn.Reserve(Bed, job, Bed.SleepingSlotsCount, 0, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnAggroMentalState(TargetIndex.A);
            this.FailOn(() => Prisoner.guest.IsInteractionEnabled(DefsOf.CE_RestrainToBed) is false);
            this.FailOn(() => Prisoner.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed));
            yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.B, TargetIndex.A);

            AddFinishAction(jobCondition =>
            {
                if (jobCondition != JobCondition.Ongoing && pawn.carryTracker.CarriedThing != null)
                {
                    pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out _);
                }
            });

            var goToPrisoner = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            var startCarry = Toils_Haul.StartCarryThing(TargetIndex.A);
            startCarry.FailOnBedNoLongerUsable(TargetIndex.B, TargetIndex.A);
            var goToBed = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOn(() => pawn.IsCarryingPawn(Prisoner) is false);
            goToBed.FailOnBedNoLongerUsable(TargetIndex.B, TargetIndex.A);
            var goToInteractionCell = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell);

            yield return Toils_Jump.JumpIf(goToBed, () => pawn.IsCarryingPawn(Prisoner));
            yield return Toils_Jump.JumpIf(goToInteractionCell, () => Prisoner.CurrentBed() == Bed);
            yield return goToPrisoner;
            yield return startCarry;
            yield return goToBed;
            yield return Toils_Reserve.Release(TargetIndex.B);
            yield return Toils_Bed.TuckIntoBed(Bed, pawn, Prisoner);
            yield return goToInteractionCell;

            var tieDown = Toils_General.WaitWith(TargetIndex.A, 180, useProgressBar: true);
            tieDown.PlaySustainerOrSound(SoundDefOf.ChainToPlatform);
            yield return tieDown;

            yield return Toils_General.Do(() =>
            {
                Prisoner.health.AddHediff(DefsOf.CE_RestrainedToBed);
                Prisoner.Drawer.renderer.SetAllGraphicsDirty();
            });
        }
    }
}
