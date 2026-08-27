using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(JobGiver_Binge), "IgnoreForbid")]
    public static class JobGiver_Binge_IgnoreForbid_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result is false || pawn.IsPrisoner is false || PrisonBreakUtility.IsPrisonBreaking(pawn) || pawn.IsCaged() is false) return;
            __result = false;
        }
    }
}
