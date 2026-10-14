using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch]
    public static class HeldPrisonerIngredientPlacementPatch
    {
        public static MethodBase TargetMethod()
        {
            MethodInfo method = AccessTools.Method(typeof(Toils_JobTransforms),
                "IngredientPlaceCellsInOrder", new[] { typeof(Thing) });
            if (method == null)
                throw new MissingMethodException("Ingredient placement iterator was not found.");
            return AccessTools.EnumeratorMoveNext(method)
                ?? throw new MissingMethodException("Ingredient placement iterator MoveNext was not found.");
        }

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
                throw new InvalidOperationException("Expected one map lookup in the ingredient placement iterator.");
            return codes;
        }
    }
}
