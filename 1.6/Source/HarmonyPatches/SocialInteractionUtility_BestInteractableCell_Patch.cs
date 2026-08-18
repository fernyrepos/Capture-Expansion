using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(SocialInteractionUtility), nameof(SocialInteractionUtility.BestInteractableCell))]
    public static class SocialInteractionUtility_BestInteractableCell_Patch
    {
        public static bool Prefix(Pawn targetPawn, ref IntVec3 __result)
        {
            if (targetPawn.Spawned is false && targetPawn.ParentHolder is Building_HoldingPlatform platform)
            {
                __result = platform.InteractionCell.IsValid ? platform.InteractionCell : platform.Position;
                return false;
            }
            return true;
        }
    }
}
