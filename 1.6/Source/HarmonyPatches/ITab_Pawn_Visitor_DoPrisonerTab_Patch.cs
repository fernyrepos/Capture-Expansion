using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoPrisonerTab")]
    public static class ITab_Pawn_Visitor_DoPrisonerTab_Patch
    {
        public static void Postfix(ITab_Pawn_Visitor __instance, Listing_Standard listing)
        {
            var selPawn = __instance.SelPawn;
            if (selPawn != null && selPawn.IsPrisoner)
            {
                var data = selPawn.GetData();
                var restrained = data.restrainedToBed;
                listing.CheckboxLabeled("CE_RestrainToBed".Translate(), ref restrained);
                data.restrainedToBed = restrained;
            }
        }
    }
}
