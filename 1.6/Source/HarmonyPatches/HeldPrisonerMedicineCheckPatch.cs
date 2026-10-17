using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), "CannotDoBillDueToMedicineRestriction")]
    public static class HeldPrisonerMedicineCheckPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var mapGetter = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Map));
            var mapHeldGetter = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.MapHeld));
            int replacements = 0;

            foreach (var code in codes)
            {
                if (!code.Calls(mapGetter))
                    continue;
                code.operand = mapHeldGetter;
                replacements++;
            }

            if (replacements != 1)
                throw new InvalidOperationException("Expected one map lookup in the medicine restriction check.");
            return codes;
        }
    }
}
