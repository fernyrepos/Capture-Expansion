using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.CompGetGizmosExtra))]
    public static class CompHoldingPlatformTarget_CompGetGizmosExtra_Patch
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> values, CompHoldingPlatformTarget __instance)
        {
            var pawn = __instance.parent as Pawn;
            var isHumanPrisoner = pawn != null && pawn.RaceProps.Humanlike && pawn.IsMutant is false && pawn.IsPrisonerOfColony;
            foreach (var v in values)
            {
                if (isHumanPrisoner && __instance.CurrentlyHeldOnPlatform is false)
                {
                    continue;
                }
                yield return v;
            }

            if (isHumanPrisoner && __instance.CurrentlyHeldOnPlatform is false)
            {
                if (__instance.targetHolder != null)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "CancelTransfer".Translate(),
                        defaultDesc = "CancelTransferDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel"),
                        action = () => __instance.targetHolder = null
                    };
                }
                else
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "TransferEntity".Translate(pawn) + "...",
                        defaultDesc = "TransferEntityDesc".Translate(pawn).Resolve(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/TransferEntity"),
                        action = () => StudyUtility.TargetHoldingPlatformForEntity(null, pawn, true, pawn.ParentHolder as Thing)
                    };
                }
            }
        }
    }
}
