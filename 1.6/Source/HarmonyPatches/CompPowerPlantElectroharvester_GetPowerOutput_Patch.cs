using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompPowerPlantElectroharvester), "GetPowerOutput")]
    public static class CompPowerPlantElectroharvester_GetPowerOutput_Patch
    {
        public static void Postfix(CompPowerPlantElectroharvester __instance, ref float __result)
        {
            foreach (var building in __instance.Platforms)
            {
                if (building is Building_HoldingPlatform { HeldPawn: { RaceProps.Humanlike: true, IsMutant: false } })
                {
                    __result = 0f;
                    return;
                }
            }
        }
    }
}
