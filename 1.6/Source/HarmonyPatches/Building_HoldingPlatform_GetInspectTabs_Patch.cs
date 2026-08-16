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
                if (held.IsPrisonerOfColony is false && held.Faction != Faction.OfPlayer)
                {
                    held.guest?.CapturedBy(Faction.OfPlayer);
                }
                yield return InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Prisoner));
            }
        }
    }
}
