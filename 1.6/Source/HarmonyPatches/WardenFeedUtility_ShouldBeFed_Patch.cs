using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WardenFeedUtility), "ShouldBeFed")]
    public static class WardenFeedUtility_ShouldBeFed_Patch
    {
        public static void Postfix(Pawn p, ref bool __result)
        {
            if (__result is false && p.IsPrisoner && PrisonBreakUtility.IsPrisonBreaking(p) is false && p.ownership?.OwnedBed != null && State.IsRestrained(p))
            {
                __result = true;
            }
        }
    }
}
