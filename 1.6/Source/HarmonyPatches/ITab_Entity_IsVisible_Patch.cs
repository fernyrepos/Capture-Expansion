using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(ITab_Entity), nameof(ITab_Entity.IsVisible), MethodType.Getter)]
    public static class ITab_Entity_IsVisible_Patch
    {
        public static void Postfix(ITab_Entity __instance, ref bool __result)
        {
            if (__result && __instance.SelPawn is { RaceProps.Humanlike: true, IsMutant: false, IsEntity: false })
            {
                __result = false;
            }
        }
    }
}
