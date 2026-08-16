using HarmonyLib;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Head), nameof(PawnRenderNodeWorker_Head.RotationFor))]
    public static class PawnRenderNodeWorker_Head_RotationFor_Patch
    {
        public static void Postfix(PawnDrawParms parms, ref Quaternion __result)
        {
            if (parms.flipHead && parms.pawn.ParentHolder is Building_RestraintPlatform { Rotation: var rot } platform && rot == Rot4.North && platform.RestraintExtension.crawlingHeadNorth == true)
            {
                __result *= 180f.ToQuat();
            }
        }
    }
}
