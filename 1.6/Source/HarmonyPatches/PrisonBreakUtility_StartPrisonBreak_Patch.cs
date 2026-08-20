using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PrisonBreakUtility), "StartPrisonBreak",
    [
        typeof(Pawn),
        typeof(string),
        typeof(string),
        typeof(LetterDef),
        typeof(List<Pawn>)
    ],
    [
        ArgumentType.Normal,
        ArgumentType.Out,
        ArgumentType.Out,
        ArgumentType.Out,
        ArgumentType.Out
    ])]
    public static class PrisonBreakUtility_StartPrisonBreak_Patch
    {
        public static void Postfix(Pawn initiator)
        {
            var hediff = initiator.health.hediffSet.GetFirstHediffOfDef(DefsOf.CE_RestrainedToBed);
            if (hediff != null)
            {
                initiator.health.RemoveHediff(hediff);
            }
            initiator.guest?.ToggleNonExclusiveInteraction(DefsOf.CE_RestrainToBed, false);
        }
    }
}
