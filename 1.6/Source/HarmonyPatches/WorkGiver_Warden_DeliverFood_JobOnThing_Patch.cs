using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "JobOnThing")]
    public static class WorkGiver_Warden_DeliverFood_JobOnThing_Patch
    {
        public static void Postfix(Thing t, ref Job __result)
        {
            if (__result != null && t is Pawn prisoner && prisoner.IsPrisoner && PrisonBreakUtility.IsPrisonBreaking(prisoner) is false && prisoner.ownership?.OwnedBed is Building_Cage cage && cage.OccupiedRect().Contains(__result.targetC.Cell) is false && cage.WanderCells().TryRandomElement(out var cell))
            {
                __result.targetC = cell;
            }
        }
    }
}
