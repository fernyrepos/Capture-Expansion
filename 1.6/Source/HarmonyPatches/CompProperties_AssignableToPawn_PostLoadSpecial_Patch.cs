using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(CompProperties_AssignableToPawn), nameof(CompProperties_AssignableToPawn.PostLoadSpecial))]
    public static class CompProperties_AssignableToPawn_PostLoadSpecial_Patch
    {
        public static void Postfix(CompProperties_AssignableToPawn __instance, ThingDef parent)
        {
            if (parent.thingClass != typeof(Building_Cage)) return;

            var area = parent.size.x * parent.size.z;
            __instance.maxAssignedPawnsCount = area switch
            {
                4 => 1,
                6 => 2,
                9 => 4,
                _ => 1
            };
            __instance.drawUnownedAssignmentOverlay = false;
        }
    }
}
