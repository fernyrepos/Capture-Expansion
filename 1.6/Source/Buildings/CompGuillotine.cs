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

        public CompProperties_Guillotine Props => (CompProperties_Guillotine)props;

        private Building_HoldingPlatform Platform => parent as Building_HoldingPlatform;

        private Texture2D Icon => cachedIcon ??= ContentFinder<Texture2D>.Get(Props.iconTexPath);

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
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
                        () => Behead(victim),
                        destructive: true));
                }
            };
        }

        private static void Behead(Pawn victim)
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
