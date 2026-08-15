using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_Feed), "JobOnThing")]
    public static class WorkGiver_Warden_Feed_JobOnThing_Patch
    {
        public static void Postfix(Pawn pawn, Thing t, ref Job __result)
        {
            if (__result == null && t is Pawn p2 && p2.RaceProps.Humanlike && p2.IsMutant is false)
            {
                var comp = p2.TryGetComp<CompHoldingPlatformTarget>();
                if (comp != null && comp.CurrentlyHeldOnPlatform && p2.needs.food != null && p2.needs.food.CurCategory >= HungerCategory.Hungry && FoodUtility.TryFindBestFoodSourceFor(pawn, p2, p2.needs.food.CurCategory == HungerCategory.Starving, out var foodSource, out var foodDef, false, true, false, false, false))
                {
                    var nutrition = FoodUtility.GetNutrition(p2, foodSource, foodDef);
                    var job = JobMaker.MakeJob(JobDefOf.FeedPatient, foodSource, p2);
                    job.count = FoodUtility.WillIngestStackCountOf(p2, foodDef, nutrition);
                    __result = job;
                }
            }
        }
    }
}
