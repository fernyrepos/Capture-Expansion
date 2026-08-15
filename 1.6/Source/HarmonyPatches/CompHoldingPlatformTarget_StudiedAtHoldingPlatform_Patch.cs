using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.StudiedAtHoldingPlatform), MethodType.Getter)]
    public static class CompHoldingPlatformTarget_StudiedAtHoldingPlatform_Patch
    {
        public static void Postfix(CompHoldingPlatformTarget __instance, ref bool __result)
        {
            if (__result is false && __instance.parent is Pawn pawn && pawn.RaceProps.Humanlike)
            {
                __result = true;
            }
        }
    }
}