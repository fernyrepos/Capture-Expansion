using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.Notify_HeldOnPlatform))]
    public static class CompHoldingPlatformTarget_Notify_HeldOnPlatform_Patch
    {
        public static void Postfix(CompHoldingPlatformTarget __instance)
        {
            if (__instance.parent is Pawn pawn && pawn.RaceProps.Humanlike && pawn.IsMutant is false)
            {
                pawn.ownership?.UnclaimBed();
                if (pawn.IsPrisonerOfColony is false)
                {
                    pawn.guest?.CapturedBy(Faction.OfPlayer);
                }
            }
        }
    }
}