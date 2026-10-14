using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch(typeof(LordJob_Ritual), "ShouldRemovePawn")]
    internal static class RitualPrisonerRestraintMembershipPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static void Postfix(LordJob_Ritual __instance, Pawn p, PawnLostCondition reason, ref bool __result)
        {
            if (reason != PawnLostCondition.MadePrisoner || !RitualExecutionUtility.Supported(__instance.Ritual)
                || !RitualExecutionUtility.DeliveryStage(__instance.CurrentStage, __instance.Ritual))
                return;
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(__instance.selectedTarget.Thing);
            if (platform != null && platform.HeldPawn == p && __instance.PawnWithRole("prisoner") == p)
                __result = false;
        }
    }
}
