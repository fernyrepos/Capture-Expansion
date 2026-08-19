using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(WorkGiver_TransferEntity), "HasJobOnThing")]
    public static class WorkGiver_TransferEntity_HasJobOnThing_Patch
    {
        public static void Postfix(Thing t, ref bool __result)
        {
            if (__result && t is Building_HoldingPlatform { HeldPawn: var held } && held != null)
            {
                var comp = held.TryGetComp<CompHoldingPlatformTarget>();
                if (comp != null && comp.targetHolder is Building_Bed && held.RaceProps.Humanlike && held.IsMutant is false)
                {
                    __result = false;
                }
            }
        }
    }
}
