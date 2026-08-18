using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith))]
    public static class Pawn_InteractionsTracker_CanInteractNowWith_Patch
    {
        public static bool Prefix(Pawn_InteractionsTracker __instance, Pawn recipient, InteractionDef interactionDef, ref bool __result)
        {
            if (recipient.Spawned || recipient.RaceProps.Humanlike is false || recipient.ParentHolder is not Building_HoldingPlatform platform)
            {
                return true;
            }
            __result = __instance.InteractedTooRecentlyToInteract() is false && SocialInteractionUtility.CanInitiateInteraction(__instance.pawn, interactionDef) && SocialInteractionUtility.CanReceiveInteraction(recipient, interactionDef) && __instance.pawn.CanReach(platform, PathEndMode.ClosestTouch, Danger.Deadly);
            return false;
        }
    }
}
