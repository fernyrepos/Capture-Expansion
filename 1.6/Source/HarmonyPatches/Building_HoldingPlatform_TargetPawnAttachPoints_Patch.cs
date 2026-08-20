using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_HoldingPlatform), "TargetPawnAttachPoints", MethodType.Getter)]
    public static class Building_HoldingPlatform_TargetPawnAttachPoints_Patch
    {
        public static void Postfix(Building_HoldingPlatform __instance, ref AttachPointTracker __result)
        {
            if (__result != null || __instance.HeldPawn is not Pawn { RaceProps.Humanlike: true } held || held.story?.bodyType?.attachPoints == null)
            {
                return;
            }
            __result = new AttachPointTracker(held.story.bodyType.attachPoints, held);
            __instance.targetPoints = __result;
        }
    }
}
