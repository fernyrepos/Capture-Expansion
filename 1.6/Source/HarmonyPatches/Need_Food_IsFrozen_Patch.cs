using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Need_Food), "IsFrozen", MethodType.Getter)]
    public static class Need_Food_IsFrozen_Patch
    {
        public static void Postfix(Need_Food __instance, ref bool __result)
        {
            if (__result && __instance.pawn.RaceProps.Humanlike && __instance.pawn.IsMutant is false)
            {
                var comp = __instance.pawn.TryGetComp<CompHoldingPlatformTarget>();
                if (comp != null && comp.CurrentlyHeldOnPlatform)
                {
                    __result = false;
                }
            }
        }
    }
}
