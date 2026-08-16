using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker), nameof(PawnRenderNodeWorker.OffsetFor))]
    public static class PawnRenderNodeWorker_OffsetFor_Patch
    {
        public static void Postfix(PawnRenderNode node, PawnDrawParms parms, ref Vector3 __result)
        {
            if (parms.pawn.ParentHolder is Building_RestraintPlatform platform)
            {
                if (node is PawnRenderNode_Head || node.Props.tagDef == PawnRenderNodeTagDefOf.Head)
                {
                    __result += platform.RestraintExtension.HeadDrawOffsetFor(platform.Rotation);
                }
                else if (node is PawnRenderNode_Body || node.Props.tagDef == PawnRenderNodeTagDefOf.Body)
                {
                    __result += platform.RestraintExtension.BodyDrawOffsetFor(platform.Rotation);
                }
            }
        }
    }
}
