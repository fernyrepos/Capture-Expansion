using HarmonyLib;
using RimWorld;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_Scanner), nameof(WorkGiver_Scanner.MaxRegionsToScanBeforeGlobalSearch), MethodType.Getter)]
    public static class WorkGiver_Warden_MaxRegionsToScan_Patch
    {
        public static void Postfix(WorkGiver_Scanner __instance, ref int __result)
        {
            if (__instance is WorkGiver_Warden && __result < 0)
            {
                __result = 1;
            }
        }
    }
}
