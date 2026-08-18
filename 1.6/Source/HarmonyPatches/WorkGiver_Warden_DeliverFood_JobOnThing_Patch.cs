using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "JobOnThing")]
    public static class WorkGiver_Warden_DeliverFood_JobOnThing_Patch
    {
        public static bool Prefix(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (t is Pawn heldPrisoner && heldPrisoner.ParentHolder is Building_HoldingPlatform)
            {
                __result = null;
                return false;
            }
            if (t is Pawn prisoner && State.IsCaged(prisoner) && prisoner.ownership?.OwnedBed is Building_Cage cage)
            {
                if (prisoner.IsPrisonerOfColony is false || prisoner.InAggroMentalState || prisoner.guest.CanBeBroughtFood is false || prisoner.needs?.food == null || prisoner.needs.food.CurLevelPercentage >= prisoner.needs.food.PercentageThreshHungry + 0.02f || WardenFeedUtility.ShouldBeFed(prisoner) || t.IsForbidden(pawn))
                {
                    return false;
                }
                if (pawn.CanReserve(prisoner, 1, -1, null, forced) is false || pawn.CanReach(cage, PathEndMode.Touch, Danger.Some) is false || FoodAvailableInCage(prisoner, cage))
                {
                    return false;
                }
                if (FoodUtility.TryFindBestFoodSourceFor(pawn, prisoner, prisoner.needs.food.CurCategory == HungerCategory.Starving, out var foodSource, out var foodDef, canRefillDispenser: false, canUseInventory: true, canUsePackAnimalInventory: false, allowForbidden: false, allowCorpse: false, allowSociallyImproper: false, allowHarvest: false, forceScanWholeMap: false, ignoreReservations: false, calculateWantedStackCount: true))
                {
                    var targetCell = cage.WanderCells.TryRandomElement(out var c) ? c : cage.Position;
                    var nutrition = FoodUtility.GetNutrition(prisoner, foodSource, foodDef);
                    var job = JobMaker.MakeJob(JobDefOf.DeliverFood, foodSource, prisoner);
                    job.count = FoodUtility.WillIngestStackCountOf(prisoner, foodDef, nutrition);
                    job.targetC = targetCell;
                    __result = job;
                }
                return false;
            }
            return true;
        }

        public static void Postfix(Thing t, ref Job __result)
        {
            if (__result != null && t is Pawn prisoner && prisoner.IsPrisoner && PrisonBreakUtility.IsPrisonBreaking(prisoner) is false && prisoner.ownership?.OwnedBed is Building_Cage cage && cage.OccupiedRect().Contains(__result.targetC.Cell) is false && cage.WanderCells.TryRandomElement(out var cell))
            {
                __result.targetC = cell;
            }
        }

        private static bool FoodAvailableInCage(Pawn prisoner, Building_Cage cage)
        {
            if (prisoner.carryTracker.CarriedThing != null && prisoner.carryTracker.CarriedThing.def.IsNutritionGivingIngestible)
            {
                return true;
            }
            var wanderCells = cage.WanderCells;
            var nutritionInCage = 0f;
            for (var i = 0; i < wanderCells.Count; i++)
            {
                var things = wanderCells[i].GetThingList(cage.Map);
                for (var j = 0; j < things.Count; j++)
                {
                    var thing = things[j];
                    if (thing.def.IsIngestible && (int)thing.def.ingestible.preferability > 3)
                    {
                        nutritionInCage += thing.GetStatValue(StatDefOf.Nutrition) * thing.stackCount;
                    }
                }
            }
            var nutritionWanted = 0f;
            foreach (var occupant in cage.CurOccupants)
            {
                if (occupant.IsPrisonerOfColony && occupant.needs?.food != null && occupant.needs.food.CurLevelPercentage < occupant.needs.food.PercentageThreshHungry + 0.02f && occupant.carryTracker.CarriedThing == null)
                {
                    nutritionWanted += occupant.needs.food.NutritionWanted;
                }
            }
            return nutritionInCage + 0.5f >= nutritionWanted;
        }
    }
}
