using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace CaptureExpansion.Compatibility.Ideology
{
    internal static class RitualExecutionUtility
    {
        internal static bool Supported(Precept_Ritual ritual)
        {
            if (!ModsConfig.IdeologyActive)
                return false;
            string pattern = ritual?.sourcePattern?.defName;
            string behavior = ritual?.behavior?.def?.defName;
            return (pattern == "SacrificePrisoner" && behavior == "SacrificePrisoner")
                || (pattern == "Execution" && behavior == "Execution");
        }

        internal static Building_RestraintPlatform Device(Thing thing)
        {
            if (!(thing is Building_RestraintPlatform platform)
                || (thing.def.defName != "CE_Guillotine" && thing.def.defName != "CE_ElectricChair"))
                return null;
            return platform;
        }

        internal static bool RitualInProgress(Building_HoldingPlatform platform)
        {
            if (!ModsConfig.IdeologyActive || platform == null || !platform.Spawned)
                return false;
            if (platform.HeldPawn?.GetLord()?.LordJob is LordJob_Ritual heldRitual
                && Supported(heldRitual.Ritual) && heldRitual.selectedTarget.Thing == platform)
                return true;
            return platform.Map.reservationManager.TryGetReserver(platform, Faction.OfPlayer, out Pawn reserver)
                && reserver.GetLord()?.LordJob is LordJob_Ritual ritual
                && Supported(ritual.Ritual) && ritual.selectedTarget.Thing == platform;
        }

        internal static string DeviceIssue(Building_RestraintPlatform platform, bool checkBusy = true)
        {
            if (platform == null || !platform.Spawned || platform.Faction != Faction.OfPlayer
                || platform.TryGetComp<CompEntityHolder>() == null)
                return "CE_RitualDeviceUnavailable".Translate();
            if (platform.IsForbidden(Faction.OfPlayer) || platform.IsBurning())
                return "CE_RitualDeviceUnsafe".Translate();
            CompGuillotine guillotine = platform.TryGetComp<CompGuillotine>();
            CompElectricChair chair = platform.TryGetComp<CompElectricChair>();
            if (guillotine == null && chair == null)
                return "CE_RitualDeviceUnavailable".Translate();
            if ((guillotine?.Activated ?? false) || (chair?.Activated ?? false))
                return "CE_RitualOrdinaryExecutionQueued".Translate();
            if (chair != null && platform.TryGetComp<CompPowerTrader>()?.PowerOn != true)
                return "CE_ActivateElectricChairNoPower".Translate();
            if (checkBusy && RitualInProgress(platform))
                return "CE_RitualExecutionBusy".Translate();
            return null;
        }

        internal static string PrisonerIssue(Pawn victim, Building_RestraintPlatform platform, bool checkBusy = true)
        {
            if (victim == null || victim.Dead || victim.Destroyed || !victim.RaceProps.Humanlike
                || !victim.IsPrisonerOfColony || victim.IsMutant)
                return "CE_RitualNeedsPrisoner".Translate();
            if (victim.MapHeld != platform.Map || !victim.guest.PrisonerIsSecure || victim.IsFormingCaravan())
                return "CE_RitualPrisonerUnavailable".Translate();
            if (checkBusy && victim.GetLord() != null)
                return "CE_RitualPrisonerBusy".Translate();
            CompHoldingPlatformTarget holdingTarget = victim.TryGetComp<CompHoldingPlatformTarget>();
            if (holdingTarget == null || holdingTarget.Props.heldPawnKind != null)
                return "CE_RitualCannotRestrain".Translate();
            return null;
        }

        internal static string OperatorRole(Precept_Ritual ritual)
        {
            return ritual.sourcePattern.defName == "Execution" ? "executioner" : "moralist";
        }

        internal static bool DeliveryStage(RitualStage stage, Precept_Ritual ritual)
        {
            return stage?.BehaviorForRole(OperatorRole(ritual))?.dutyDef?.defName == "DeliverPawnToAltar";
        }

        internal static bool CompatibleStages(Precept_Ritual ritual)
        {
            List<RitualStage> stages = ritual.behavior.def.stages;
            if (stages == null || stages.Count < 2 || !DeliveryStage(stages[0], ritual))
                return false;
            if (stages[0].endTriggers == null
                || !stages[0].endTriggers.Any(trigger => trigger is StageEndTrigger_PawnDeliveredOrNotValid))
                return false;
            string operatorRole = OperatorRole(ritual);
            for (int i = 1; i < stages.Count; i++)
                if (stages[i]?.BehaviorForRole(operatorRole)?.dutyDef?.defName == "Sacrifice")
                    return true;
            return false;
        }

        internal static string StartIssue(Precept_Ritual ritual, Building_RestraintPlatform platform,
            Dictionary<string, Pawn> forced = null)
        {
            string issue = DeviceIssue(platform);
            if (issue != null)
                return issue;
            if (!CompatibleStages(ritual))
                return "CE_RitualIncompatibleStages".Translate();
            RitualRole role = ritual.behavior.def.roles?.FirstOrDefault(r => r.id == "prisoner");
            if (role == null)
                return "CE_RitualMissingPrisonerRole".Translate();
            Pawn victim = platform.HeldPawn;
            if (forced != null && forced.TryGetValue("prisoner", out Pawn forcedVictim))
            {
                if (victim != null && forcedVictim != victim)
                    return "CE_RitualPrisonerChanged".Translate();
                victim = forcedVictim;
                if (victim == null)
                    return "CE_RitualNeedsPrisoner".Translate();
            }
            if (victim == null)
                return null;
            issue = PrisonerIssue(victim, platform);
            if (issue != null)
                return issue;
            if (!victim.Spawned && victim.ParentHolder != platform)
                return "CE_RitualPrisonerUnavailable".Translate();
            return RitualRoleAssignments.PawnNotAssignableReason(victim, role, ritual, null, platform, out _);
        }

        internal static Dictionary<string, Pawn> ForcePrisoner(Dictionary<string, Pawn> forced, Pawn victim)
        {
            if (victim == null || (forced != null && forced.ContainsKey("prisoner")))
                return forced;
            var result = forced == null ? new Dictionary<string, Pawn>() : new Dictionary<string, Pawn>(forced);
            result.Add("prisoner", victim);
            return result;
        }

        internal static bool ActiveTargetAllowed(Building_RestraintPlatform platform, LordJob_Ritual ritual)
        {
            if (!platform.Spawned || platform.Faction != Faction.OfPlayer)
                return false;
            Pawn victim = ritual.PawnWithRole("prisoner");
            if (victim == null)
                return false;
            if (victim.Dead)
                return ritual.pawnsDeathIgnored.Contains(victim);
            if (DeviceIssue(platform, checkBusy: false) != null
                || PrisonerIssue(victim, platform, checkBusy: false) != null || victim.GetLord() != ritual.lord)
                return false;
            if (platform.HeldPawn == victim)
                return true;
            if (platform.Occupied || !DeliveryStage(ritual.CurrentStage, ritual.Ritual))
                return false;
            if (victim.Spawned)
                return true;
            return victim.ParentHolder is Pawn_CarryTracker carrier
                && carrier.pawn == ritual.PawnWithRole(OperatorRole(ritual.Ritual));
        }
    }
}
