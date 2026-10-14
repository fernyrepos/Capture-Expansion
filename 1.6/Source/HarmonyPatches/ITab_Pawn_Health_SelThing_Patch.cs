using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ITab), nameof(ITab.SelThing), MethodType.Getter)]
    public static class ITab_Pawn_Health_SelThing_Patch
    {
        public static void Postfix(ITab __instance, ref Thing __result)
        {
            if (__instance is ITab_Pawn_Health && __result is Building_RestraintPlatform platform
                && platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false } pawn)
                __result = pawn;
        }
    }
}
