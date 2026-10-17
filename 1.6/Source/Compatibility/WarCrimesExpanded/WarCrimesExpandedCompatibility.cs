using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion.Compatibility.WarCrimesExpanded
{
    [StaticConstructorOnStartup]
    internal static class WarCrimesExpandedCompatibility
    {
        internal static PrisonerInteractionModeDef MutilateMode;
        internal static Action<Pawn, bool> CreateBill;

        static WarCrimesExpandedCompatibility()
        {
            if (!ModsConfig.IsActive("Mersid.WCE2Updated.Core"))
                return;
            Type patches = AccessTools.TypeByName("WarCrimesExpanded2.HarmonyPatches");
            Type utility = AccessTools.TypeByName("WarCrimesExpanded2.RandomMutilationUtility");
            MethodInfo createBill = utility == null ? null : AccessTools.Method(utility,
                "CreateRandomMutilationBill", new[] { typeof(Pawn), typeof(bool) });
            MethodInfo tick = patches == null ? null : AccessTools.Method(patches, "GuestTrackerTick",
                new[] { typeof(Pawn_GuestTracker), typeof(Pawn), typeof(GuestStatus), typeof(PrisonerInteractionModeDef) });
            MethodInfo modeChanged = patches == null ? null : AccessTools.Method(patches,
                "InteractionModeChanged", new[] { typeof(PrisonerInteractionModeDef) });
            MutilateMode = DefDatabase<PrisonerInteractionModeDef>.GetNamedSilentFail("WCE2_Mutilate");
            if (createBill == null || !createBill.IsStatic || createBill.ReturnType != typeof(void)
                || tick == null || !tick.IsStatic || tick.ReturnType != typeof(void)
                || modeChanged == null || !modeChanged.IsStatic || modeChanged.ReturnType != typeof(void)
                || MutilateMode == null)
            {
                Log.Error("[Capture Expansion] War Crimes Expanded compatibility could not be installed: expected methods or WCE2_Mutilate are missing.");
                return;
            }
            CreateBill = (Action<Pawn, bool>)Delegate.CreateDelegate(typeof(Action<Pawn, bool>), createBill);
            CaptureExpansionMod.harmony.Patch(tick, postfix: new HarmonyMethod(
                typeof(HeldPrisonerMutilationPatch), nameof(HeldPrisonerMutilationPatch.Postfix)));
            CaptureExpansionMod.harmony.Patch(modeChanged, prefix: new HarmonyMethod(
                typeof(PrisonerInteractionModePatch), nameof(PrisonerInteractionModePatch.Prefix)));
        }

        internal static bool Eligible(Pawn pawn)
        {
            return pawn != null && !pawn.Spawned && !pawn.Dead && !pawn.Destroyed
                && pawn.IsPrisonerOfColony && pawn.RaceProps.Humanlike && !pawn.IsMutant
                && pawn.ParentHolder is Building_RestraintPlatform platform
                && platform.Spawned && platform.Faction == Faction.OfPlayer && platform.HeldPawn == pawn
                && pawn.BillStack != null && pawn.BillStack.Count == 0;
        }
    }
}
