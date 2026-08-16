using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Toils_Ingest), nameof(Toils_Ingest.CarryIngestibleToChewSpot))]
    public static class Toils_Ingest_CarryIngestibleToChewSpot_Patch
    {
        public static bool Prefix(Pawn pawn, ref Toil __result)
        {
            if (State.IsCaged(pawn) && pawn.ownership?.OwnedBed is Building_Cage cage)
            {
                var toil = ToilMaker.MakeToil("CarryIngestibleToChewSpot_Cage");
                toil.initAction = () =>
                {
                    var cells = cage.WanderCells;
                    var cell = cells.Contains(toil.actor.Position) ? toil.actor.Position : cells.TryRandomElement(out var c) ? c : toil.actor.Position;
                    toil.actor.pather.StartPath(cell, PathEndMode.OnCell);
                };
                toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
                __result = toil;
                return false;
            }
            return true;
        }
    }
}