using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobDriver_UnrestrainPrisonerFromBed : JobDriver
    {
        protected Pawn Prisoner => (Pawn)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Prisoner.ClearAllReservations();
            return pawn.Reserve(Prisoner, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.A);
            this.FailOn(() => Prisoner.guest.IsInteractionEnabled(DefsOf.CE_RestrainToBed));
            this.FailOn(() => Prisoner.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed) is false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true);
            yield return Toils_General.Do(() =>
            {
                var hediff = Prisoner.health.hediffSet.GetFirstHediffOfDef(DefsOf.CE_RestrainedToBed);
                if (hediff != null)
                {
                    Prisoner.health.RemoveHediff(hediff);
                }
            });
        }
    }
}
