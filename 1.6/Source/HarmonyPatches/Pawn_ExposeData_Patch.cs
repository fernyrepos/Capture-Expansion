using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExposeData))]
    public static class Pawn_ExposeData_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.RaceProps.Humanlike)
            {
                __instance.GetData().ExposeData();
            }
        }
    }
}
