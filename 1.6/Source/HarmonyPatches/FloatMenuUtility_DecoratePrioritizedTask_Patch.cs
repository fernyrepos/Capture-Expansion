using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(FloatMenuUtility), nameof(FloatMenuUtility.DecoratePrioritizedTask))]
    public static class FloatMenuUtility_DecoratePrioritizedTask_Patch
    {
        public static void Postfix(FloatMenuOption __result)
        {
            if (__result.revalidateClickTarget is Pawn { Spawned: false, ParentHolder: Building_HoldingPlatform platform })
            {
                __result.revalidateClickTarget = platform;
            }
        }
    }
}
