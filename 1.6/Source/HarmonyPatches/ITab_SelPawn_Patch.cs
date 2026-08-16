using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ITab), nameof(ITab.SelPawn), MethodType.Getter)]
    public static class ITab_SelPawn_Patch
    {
        public static void Postfix(ITab __instance, ref Pawn __result)
        {
            if (__result == null && __instance.SelThing is Building_HoldingPlatform platform)
            {
                __result = platform.HeldPawn;
            }
        }
    }
}