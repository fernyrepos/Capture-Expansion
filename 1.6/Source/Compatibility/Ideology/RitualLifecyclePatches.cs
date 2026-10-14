using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch(typeof(Precept_Ritual), "GetRitualBeginWindow")]
    internal static class RitualBeginWindowPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static bool Prefix(Precept_Ritual __instance, TargetInfo targetInfo,
            ref Dictionary<string, Pawn> forcedForRole, ref Window __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(targetInfo.Thing);
            if (platform == null || !RitualExecutionUtility.Supported(__instance))
                return true;
            string issue = RitualExecutionUtility.StartIssue(__instance, platform, forcedForRole);
            if (issue == null && Find.IdeoManager.GetActiveRitualOn(platform) != null)
                issue = "CE_RitualExecutionBusy".Translate();
            if (issue != null)
            {
                Messages.Message(issue, platform, MessageTypeDefOf.RejectInput, false);
                __result = null;
                return false;
            }
            forcedForRole = RitualExecutionUtility.ForcePrisoner(forcedForRole, platform.HeldPawn);
            return true;
        }
    }

    [HarmonyPatch(typeof(RitualBehaviorWorker), "CanStartRitualNow")]
    internal static class RitualCanStartPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static bool Prefix(TargetInfo target, Precept_Ritual ritual,
            ref Dictionary<string, Pawn> forcedForRole, ref string __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(target.Thing);
            if (platform == null || !RitualExecutionUtility.Supported(ritual))
                return true;
            string issue = RitualExecutionUtility.StartIssue(ritual, platform, forcedForRole);
            if (issue != null)
            {
                __result = issue;
                return false;
            }
            forcedForRole = RitualExecutionUtility.ForcePrisoner(forcedForRole, platform.HeldPawn);
            return true;
        }
    }

    [HarmonyPatch(typeof(RitualBehaviorWorker), "TryExecuteOn")]
    internal static class RitualExecuteValidationPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static bool Prefix(TargetInfo target, Precept_Ritual ritual, RitualRoleAssignments assignments)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(target.Thing);
            if (platform == null || !RitualExecutionUtility.Supported(ritual))
                return true;
            string issue = RitualExecutionUtility.StartIssue(ritual, platform, assignments.ForcedRolesForReading);
            if (issue == null && Find.IdeoManager.GetActiveRitualOn(platform) != null)
                issue = "CE_RitualExecutionBusy".Translate();
            Pawn victim = issue == null ? assignments.AssignedPawns("prisoner").FirstOrDefault() : null;
            if (issue == null)
                issue = RitualExecutionUtility.PrisonerIssue(victim, platform);
            if (issue == null && platform.HeldPawn != null && platform.HeldPawn != victim)
                issue = "CE_RitualPrisonerChanged".Translate();
            if (issue == null && !victim.Spawned && victim.ParentHolder != platform)
                issue = "CE_RitualPrisonerUnavailable".Translate();
            if (issue == null)
                return true;
            Messages.Message(issue, platform, MessageTypeDefOf.RejectInput, false);
            return false;
        }
    }

    [HarmonyPatch(typeof(LordJob_Ritual), "TargetStillAllowed")]
    internal static class RitualActiveTargetPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static bool Prefix(LordJob_Ritual __instance, ref bool __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(__instance.selectedTarget.Thing);
            if (platform != null && RitualExecutionUtility.Supported(__instance.Ritual)
                && !RitualExecutionUtility.ActiveTargetAllowed(platform, __instance))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
