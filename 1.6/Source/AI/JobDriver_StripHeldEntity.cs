using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class JobDriver_StripHeldEntity : JobDriver
    {
        private const int StripTicks = 60;

        private Thing Platform => TargetThingA;

        private Pawn InnerPawn => (Platform as Building_HoldingPlatform)?.HeldPawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Platform, job, 1, -1, null, errorOnFailed);
        }

        public override string GetReport()
        {
            return InnerPawn != null ? "CE_StripHeldEntity".Translate(InnerPawn.LabelCap) : base.GetReport();
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() => InnerPawn == null || InnerPawn.Destroyed || InnerPawn.RaceProps.Humanlike is false);
            var toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = () => pawn.pather.StartPath(Platform, PathEndMode.ClosestTouch);
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return toil;
            yield return Toils_General.Wait(StripTicks).WithProgressBarToilDelay(TargetIndex.A);
            var toil2 = ToilMaker.MakeToil("MakeNewToils");
            toil2.initAction = () =>
            {
                InnerPawn.Strip();
                pawn.records.Increment(RecordDefOf.BodiesStripped);
            };
            toil2.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil2;
        }
    }
}
