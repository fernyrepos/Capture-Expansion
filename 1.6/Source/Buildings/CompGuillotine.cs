using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class CompProperties_Guillotine : CompProperties
    {
        public string iconTexPath;

        public CompProperties_Guillotine()
        {
            compClass = typeof(CompGuillotine);
        }
    }

    public class CompGuillotine : ThingComp
    {
        private Texture2D cachedIcon;

        public bool Activated;

        public CompProperties_Guillotine Props => (CompProperties_Guillotine)props;

        private Building_HoldingPlatform Platform => parent as Building_HoldingPlatform;

        private Texture2D Icon => cachedIcon ??= ContentFinder<Texture2D>.Get(Props.iconTexPath);

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref Activated, "guillotineActivated");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (Activated)
            {
                yield return new Command_Action
                {
                    defaultLabel = "CE_CancelGuillotine".Translate(),
                    defaultDesc = "CE_CancelGuillotineDesc".Translate(),
                    icon = Icon,
                    action = () => Activated = false
                };
                yield break;
            }

            var victim = Platform?.HeldPawn;
            yield return new Command_Action
            {
                defaultLabel = "CE_ActivateGuillotine".Translate(),
                defaultDesc = "CE_ActivateGuillotineDesc".Translate(),
                icon = Icon,
                disabled = victim == null,
                disabledReason = "CE_ActivateGuillotineNoVictim".Translate(),
                action = () =>
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        "CE_ActivateGuillotineConfirm".Translate(victim.LabelShortCap),
                        () => Activated = true,
                        destructive: true));
                }
            };
        }

        public static void Behead(Pawn victim)
        {
            if (victim == null || victim.Dead)
            {
                return;
            }

            var headPart = victim.health.hediffSet.GetNotMissingParts()
                .FirstOrDefault(part => part.def == BodyPartDefOf.Head);
            var dinfo = new DamageInfo(DamageDefOf.ExecutionCut, 99999f, 999f, -1f, null, headPart);
            victim.TakeDamage(dinfo);
            if (!victim.Dead)
            {
                victim.Kill(dinfo);
            }
        }
    }
}
