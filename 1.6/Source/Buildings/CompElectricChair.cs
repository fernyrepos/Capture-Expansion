using System.Collections.Generic;
using CaptureExpansion.Compatibility.Ideology;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class CompProperties_ElectricChair : CompProperties
    {
        public string iconTexPath;
        public CompProperties_ElectricChair() { compClass = typeof(CompElectricChair); }
    }

    public class CompElectricChair : ThingComp
    {
        private Texture2D cachedIcon;
        public bool Activated;
        public CompProperties_ElectricChair Props => (CompProperties_ElectricChair)props;
        private Building_HoldingPlatform Platform => parent as Building_HoldingPlatform;
        private Texture2D Icon => cachedIcon ??= ContentFinder<Texture2D>.Get(Props.iconTexPath);

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref Activated, "electricChairActivated");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;

            var powerComp = parent.GetComp<CompPowerTrader>();

            if (Activated)
            {
                yield return new Command_Action
                {
                    defaultLabel = "CE_CancelElectricChair".Translate(),
                    defaultDesc = "CE_CancelElectricChairDesc".Translate(),
                    icon = Icon,
                    action = () => Activated = false
                };
                yield break;
            }

            var victim = Platform?.HeldPawn;
            var noPower = powerComp is { PowerOn: false };
            bool ritualInProgress = victim != null && !noPower && RitualExecutionUtility.RitualInProgress(Platform);
            yield return new Command_Action
            {
                defaultLabel = "CE_ActivateElectricChair".Translate(),
                defaultDesc = "CE_ActivateElectricChairDesc".Translate(),
                icon = Icon,
                disabled = victim == null || noPower || ritualInProgress,
                disabledReason = ritualInProgress ? "CE_RitualExecutionBusy".Translate() : noPower ? "CE_ActivateElectricChairNoPower".Translate() : "CE_ActivateElectricChairNoVictim".Translate(),
                action = () =>
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        "CE_ActivateElectricChairConfirm".Translate(victim.LabelShortCap),
                        () =>
                        {
                            if (RitualExecutionUtility.RitualInProgress(Platform))
                                Messages.Message("CE_RitualExecutionBusy".Translate(), parent,
                                    MessageTypeDefOf.RejectInput, false);
                            else
                                Activated = true;
                        },
                        destructive: true));
                }
            };
        }

        public static void Electrocute(Building_HoldingPlatform platform, Pawn victim)
        {
            if (victim == null || victim.Dead) return;
            platform?.EjectContents();
            var dinfo = new DamageInfo(DefsOf.CE_ExecutionElectrocution, 99999f, 999f, -1f, null, null);
            victim.TakeDamage(dinfo);
            if (!victim.Dead) victim.Kill(dinfo);
        }
    }
}
