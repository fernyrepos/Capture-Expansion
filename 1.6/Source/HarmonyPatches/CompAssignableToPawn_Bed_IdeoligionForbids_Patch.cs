using HarmonyLib;
using RimWorld;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompAssignableToPawn_Bed), nameof(CompAssignableToPawn_Bed.IdeoligionForbids))]
    public static class CompAssignableToPawn_Bed_IdeoligionForbids_Patch
    {
        public static bool Prefix(CompAssignableToPawn_Bed __instance, ref bool __result)
        {
            if (__instance.parent is Building_Cage)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
