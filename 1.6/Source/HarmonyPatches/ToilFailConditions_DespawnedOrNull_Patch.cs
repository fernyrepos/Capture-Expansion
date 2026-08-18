using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ToilFailConditions), nameof(ToilFailConditions.DespawnedOrNull))]
    public static class ToilFailConditions_DespawnedOrNull_Patch
    {
        public static void Postfix(LocalTargetInfo target, Pawn actor, ref bool __result)
        {
            if (__result && target.Thing is Pawn { Spawned: false } pawn && pawn.ParentHolder is Building_HoldingPlatform platform && platform.Spawned && platform.Map == actor.Map)
            {
                __result = false;
            }
        }
    }
}
