using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(FloatMenuOptionProvider_WorkGivers), "GetWorkGiverOption")]
    public static class FloatMenuOptionProvider_WorkGivers_GetWorkGiverOption_Patch
    {
        public static void Prefix(WorkGiverDef workGiver, ref LocalTargetInfo target)
        {
            if (target.Thing is Building_HoldingPlatform platform && platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false } held && workGiver.Worker is WorkGiver_Scanner scanner && (scanner is WorkGiver_Warden or WorkGiver_DoBill || scanner.PotentialWorkThingRequest.Accepts(held)))
            {
                target = held;
            }
        }
    }
}
