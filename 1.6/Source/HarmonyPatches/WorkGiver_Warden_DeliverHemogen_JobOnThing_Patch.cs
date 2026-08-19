using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverHemogen), "JobOnThing")]
    public static class WorkGiver_Warden_DeliverHemogen_JobOnThing_Patch
    {
        public static bool Prefix(Pawn pawn, Thing t, ref Job __result)
        {
            if (t is Pawn prisoner && prisoner.ParentHolder is Building_HoldingPlatform platform)
            {
                if (prisoner.guest.CanBeBroughtFood is false || WardenFeedUtility.ShouldBeFed(prisoner))
                {
                    __result = null;
                    return false;
                }
                if (prisoner.genes?.GetGene(GeneDefOf.Hemogenic) is not Gene_Hemogen gene_Hemogen || gene_Hemogen.hemogenPacksAllowed is false || gene_Hemogen.ShouldConsumeHemogenNow() is false)
                {
                    __result = null;
                    return false;
                }
                if (prisoner.carryTracker.CarriedCount(ThingDefOf.HemogenPack) > 0 || prisoner.inventory.Count(ThingDefOf.HemogenPack) > 0)
                {
                    __result = null;
                    return false;
                }
                var room = platform.GetRoom();
                if (room != null && room.Regions.Any(r => r.ListerThings.ThingsOfDef(ThingDefOf.HemogenPack).Count > 0))
                {
                    __result = null;
                    return false;
                }
                var thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.HemogenPack), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, pack => pack.IsForbidden(pawn) is false && pawn.CanReserve(pack) && pack.GetRoom() != room);
                if (thing == null)
                {
                    __result = null;
                    return false;
                }
                var job = JobMaker.MakeJob(JobDefOf.DeliverFood, thing, prisoner);
                job.count = 1;
                job.targetC = platform.InteractionCell.IsValid ? platform.InteractionCell : platform.Position;
                __result = job;
                return false;
            }
            return true;
        }
    }
}
