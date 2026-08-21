using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class WorkGiver_Warden_ActivateGuillotine : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.EntityHolder);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_HoldingPlatform platform || platform.GetComp<CompGuillotine>() is not { Activated: true } || platform.HeldPawn is not { Dead: false } victim)
            {
                return false;
            }
            return pawn.CanReserveAndReach(platform, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced) && pawn.CanReserve(victim, 1, -1, null, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building_HoldingPlatform platform || platform.GetComp<CompGuillotine>() is not { Activated: true } || platform.HeldPawn is not { Dead: false } victim)
            {
                return null;
            }
            return JobMaker.MakeJob(DefsOf.CE_ActivateGuillotine, platform, victim);
        }
    }
}
