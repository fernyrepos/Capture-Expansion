using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.DynamicDrawPhaseAt), new Type[] {
        typeof(DrawPhase),
        typeof(Vector3),
        typeof(bool)
    })]
    public static class Thing_DynamicDrawPhaseAt_Patch
    {
        public static void Postfix(Thing __instance, DrawPhase phase)
        {
            if (phase != DrawPhase.Draw || __instance is not Building_Bed bed || bed is Building_Cage) return;
            for (var i = 0; i < bed.SleepingSlotsCount; i++)
            {
                var occupant = bed.GetCurOccupant(i);
                if (occupant != null && occupant.TryGetData(out var data) && data.restrainedToBed)
                {
                    var slotPos = bed.GetSleepingSlotPos(i).ToVector3Shifted();
                    var altitude = AltitudeLayer.PawnUnused.AltitudeFor();
                    var rot = bed.Rotation;
                    var pTL = (slotPos + new Vector3(-0.35f, 0f, 0.45f).RotatedBy(rot)).WithY(altitude);
                    var pBR = (slotPos + new Vector3(0.35f, 0f, -0.45f).RotatedBy(rot)).WithY(altitude);
                    var pTR = (slotPos + new Vector3(0.35f, 0f, 0.45f).RotatedBy(rot)).WithY(altitude);
                    var pBL = (slotPos + new Vector3(-0.35f, 0f, -0.45f).RotatedBy(rot)).WithY(altitude);

                    GenDraw.DrawLineBetween(pTL, pBR, MaterialPool.MatFrom("Things/Building/HoldingPlatform/HoldingPlatform_EntityRope", ShaderDatabase.Cutout), 0.12f);
                    GenDraw.DrawLineBetween(pTR, pBL, MaterialPool.MatFrom("Things/Building/HoldingPlatform/HoldingPlatform_EntityRope", ShaderDatabase.Cutout), 0.12f);
                }
            }
        }
    }
}
