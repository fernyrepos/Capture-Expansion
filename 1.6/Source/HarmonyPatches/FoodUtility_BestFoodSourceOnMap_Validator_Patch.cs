using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch]
    public static class FoodUtility_BestFoodSourceOnMap_Validator_Patch
    {
        public static MethodBase TargetMethod()
        {
            var nestedTypes = typeof(FoodUtility).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var nestedType in nestedTypes)
            {
                if (nestedType.Name.Contains("c__DisplayClass") is false) continue;
                var method = nestedType.GetMethod("<BestFoodSourceOnMap>b__0", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    return method;
                }
            }
            Log.Error("Capture Expansion: FoodUtility.BestFoodSourceOnMap validator not found.");
            return null;
        }

        public static void Postfix(Thing t, Pawn ___getter, ref bool __result)
        {
            if (__result is false || ___getter.IsPrisoner is false || PrisonBreakUtility.IsPrisonBreaking(___getter) || ___getter.ownership?.OwnedBed is not Building_Cage cage || cage.OccupiedRect().Contains(t.PositionHeld)) return;
            __result = false;
        }
    }
}
