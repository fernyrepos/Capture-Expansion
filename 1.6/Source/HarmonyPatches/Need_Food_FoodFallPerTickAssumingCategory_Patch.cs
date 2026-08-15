using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Need_Food), "FoodFallPerTickAssumingCategory")]
    public static class Need_Food_FoodFallPerTickAssumingCategory_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var prop = AccessTools.PropertyGetter(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.CurrentlyHeldOnPlatform));
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(prop))
                {
                    codes[i].operand = AccessTools.Method(typeof(Need_Food_FoodFallPerTickAssumingCategory_Patch), nameof(IsHeldOnPlatformAndNotHuman));
                }
            }
            return codes;
        }

        public static bool IsHeldOnPlatformAndNotHuman(CompHoldingPlatformTarget comp)
        {
            if (comp.CurrentlyHeldOnPlatform is false) return false;
            if (comp.parent is Pawn pawn && pawn.RaceProps.Humanlike && pawn.IsMutant is false) return false;
            return true;
        }
    }
}
