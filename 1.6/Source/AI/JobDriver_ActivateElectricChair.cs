using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace CaptureExpansion
{
    public class JobDriver_ActivateElectricChair : JobDriver
    {
        private const int ExecutionDuration = 240;
        protected Building_HoldingPlatform Platform => (Building_HoldingPlatform)job.GetTarget(TargetIndex.A).Thing;
        protected Pawn Victim => (Pawn)job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Platform, job, 1, -1, null, errorOnFailed) is false) return false;
            return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOn(() => Platform.HeldPawn != Victim);
            this.FailOn(() => Platform.GetComp<CompElectricChair>() is not { Activated: true });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.WaitWith(TargetIndex.A, ExecutionDuration, useProgressBar: true);

            yield return Toils_General.Do(() =>
            {
                var platform = Platform;
                var victim = Victim;
                CompElectricChair.Electrocute(platform, victim);
                DefsOf.CE_ElectricChairActivate.PlayOneShot(SoundInfo.InMap(new TargetInfo(platform.Position, platform.Map)));
                platform.GetComp<CompElectricChair>().Activated = false;
            });
        }
    }
}
