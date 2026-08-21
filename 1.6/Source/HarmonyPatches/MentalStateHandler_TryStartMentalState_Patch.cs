using HarmonyLib;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(MentalStateHandler), nameof(MentalStateHandler.TryStartMentalState))]
    public static class MentalStateHandler_TryStartMentalState_Patch
    {
        public static bool Prefix(MentalStateHandler __instance, ref bool __result)
        {
            if (__instance.pawn.IsRestrained())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
