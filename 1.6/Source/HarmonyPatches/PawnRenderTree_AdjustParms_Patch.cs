using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderTree), "AdjustParms")]
    public static class PawnRenderTree_AdjustParms_Patch
    {
        public static void Postfix(ref PawnDrawParms parms)
        {
            if (parms.pawn.ParentHolder is Building_RestraintPlatform { Rotation: var rot } platform && rot == Rot4.North && platform.RestraintExtension.crawlingHeadNorth == true)
            {
                parms.flipHead = true;
            }
        }
    }
}
