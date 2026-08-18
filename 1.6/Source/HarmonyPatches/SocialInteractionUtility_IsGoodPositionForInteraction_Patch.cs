using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(SocialInteractionUtility), nameof(SocialInteractionUtility.IsGoodPositionForInteraction), new Type[] {
        typeof(Pawn),
        typeof(Pawn)
    })]
    public static class SocialInteractionUtility_IsGoodPositionForInteraction_Patch
    {
        public static bool Prefix(Pawn p, Pawn recipient, ref bool __result)
        {
            if (recipient.Spawned is false && recipient.ParentHolder is Building_HoldingPlatform platform)
            {
                __result = SocialInteractionUtility.IsGoodPositionForInteraction(p.Position, platform.Position, p.Map);
                return false;
            }
            return true;
        }
    }
}
