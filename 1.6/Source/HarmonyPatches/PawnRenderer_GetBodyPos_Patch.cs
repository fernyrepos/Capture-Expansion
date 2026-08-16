using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderer), "GetBodyPos")]
    public static class PawnRenderer_GetBodyPos_Patch
    {
        public static void Postfix(PawnRenderer __instance, ref Vector3 __result, ref bool showBody)
        {
            var pawn = __instance.pawn;
            if (pawn.InBed() && State.IsRestrained(pawn))
            {
                showBody = true;
                var bed = pawn.CurrentBed();
                if (bed != null && bed is not Building_Cage)
                {
                    var altLayer = (AltitudeLayer)Mathf.Max((int)bed.def.altitudeLayer, 20);
                    __result = bed.DrawPos.WithY(altLayer.AltitudeFor());
                }
            }
        }
    }
}