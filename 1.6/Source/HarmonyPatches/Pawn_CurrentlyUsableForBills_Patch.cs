using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.CurrentlyUsableForBills))]
    public static class Pawn_CurrentlyUsableForBills_Patch
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__result is false && State.IsRestrained(__instance))
            {
                __result = true;
            }
        }
    }
}
