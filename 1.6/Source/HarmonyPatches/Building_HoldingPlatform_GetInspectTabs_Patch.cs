using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Building_HoldingPlatform), nameof(Building_HoldingPlatform.GetInspectTabs))]
    public static class Building_HoldingPlatform_GetInspectTabs_Patch
    {
        public static IEnumerable<InspectTabBase> Postfix(IEnumerable<InspectTabBase> values, Building_HoldingPlatform __instance)
        {
            foreach (var tab in values)
            {
                yield return tab;
            }
            if (__instance.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false } held)
            {
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Health));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Needs));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Character));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Social));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Prisoner));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Gear));
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Log));
            }
        }
    }
}
