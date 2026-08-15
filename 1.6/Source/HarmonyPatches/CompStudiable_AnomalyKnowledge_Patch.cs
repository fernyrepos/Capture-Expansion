using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompStudiable), nameof(CompStudiable.AnomalyKnowledge), MethodType.Getter)]
    public static class CompStudiable_AnomalyKnowledge_Patch
    {
        public static void Postfix(CompStudiable __instance, ref float __result)
        {
            if (__instance.parent is Pawn pawn && pawn.RaceProps.Humanlike && pawn.IsMutant is false)
            {
                __result = 0f;
            }
        }
    }
}
