using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch(typeof(StageEndTrigger_PawnDeliveredOrNotValid), "MakeTrigger")]
    internal static class RitualPrisonerDeliveryStagePatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static void Postfix(LordJob_Ritual ritual, TargetInfo spot, RitualStage stage, ref Trigger __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(spot.Thing);
            if (platform == null || !RitualExecutionUtility.Supported(ritual.Ritual)
                || !RitualExecutionUtility.DeliveryStage(stage, ritual.Ritual))
                return;
            __result = new Trigger_TickCondition(() =>
                platform.HeldPawn != null && platform.HeldPawn == ritual.PawnWithRole("prisoner"));
        }
    }
}
