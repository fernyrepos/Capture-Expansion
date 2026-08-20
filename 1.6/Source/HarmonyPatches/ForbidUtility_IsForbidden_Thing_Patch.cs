using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ForbidUtility), "IsForbidden", typeof(Thing), typeof(Pawn))]
    public static class ForbidUtility_IsForbidden_Thing_Patch
    {
        public static void Postfix(Thing t, Pawn pawn, ref bool __result)
        {
            if (!(__result || pawn.IsPrisoner is false || PrisonBreakUtility.IsPrisonBreaking(pawn) || pawn.ownership?.OwnedBed == null) && pawn.IsCaged() && pawn.ownership.OwnedBed is Building_Cage cage && cage.WanderCells.Contains(t.Position) is false)
            {
                __result = true;
            }
        }
    }
}
