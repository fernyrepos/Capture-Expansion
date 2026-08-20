using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ForbidUtility), "IsForbidden", typeof(IntVec3), typeof(Pawn))]
    public static class ForbidUtility_IsForbidden_Cell_Patch
    {
        public static void Postfix(IntVec3 c, Pawn pawn, ref bool __result)
        {
            if (!(__result || pawn.IsPrisoner is false || PrisonBreakUtility.IsPrisonBreaking(pawn) || pawn.ownership?.OwnedBed == null) && pawn.IsCaged() && pawn.ownership.OwnedBed is Building_Cage cage && cage.WanderCells.Contains(c) is false)
            {
                __result = true;
            }
        }
    }
}
