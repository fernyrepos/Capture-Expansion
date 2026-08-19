using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ITab_Pawn_Needs), nameof(ITab_Pawn_Needs.IsVisible), MethodType.Getter)]
    public static class ITab_Pawn_Needs_IsVisible_Patch
    {
        public static void Postfix(ITab_Pawn_Needs __instance, ref bool __result)
        {
            var pawn = __instance.SelPawn;
            if (pawn is { RaceProps.Humanlike: true, IsMutant: false } && pawn.TryGetComp<CompHoldingPlatformTarget>()?.CurrentlyHeldOnPlatform == true)
            {
                __result = true;
            }
        }
    }
}
