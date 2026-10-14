using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.InteractionCell), MethodType.Getter)]
    public static class HeldPrisonerWorkCellPatch
    {
        public static bool Prefix(Pawn __instance, ref IntVec3 __result)
        {
            if (__instance.Spawned || !__instance.RaceProps.Humanlike || __instance.IsMutant
                || __instance.ParentHolder is not Building_RestraintPlatform platform
                || platform.HeldPawn != __instance)
                return true;
            __result = platform.Spawned ? platform.InteractionCell : IntVec3.Invalid;
            return false;
        }
    }
}
