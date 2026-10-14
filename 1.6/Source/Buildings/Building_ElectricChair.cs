using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class Building_ElectricChair : Building_RestraintPlatform
    {
        private ElectricChairExtension cachedExtension;
        private Graphic headCoverGraphic;

        private ElectricChairExtension Extension => cachedExtension ??= def.GetModExtension<ElectricChairExtension>();

        private Graphic HeadCoverGraphic => headCoverGraphic ??= Extension.headCoverGraphicData.Graphic
            .GetColoredVersion(Extension.headCoverGraphicData.Graphic.Shader, ThingDefOf.Steel.stuffProps.color, ThingDefOf.Steel.stuffProps.color);

        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            base.DynamicDrawPhaseAt(phase, drawLoc, flip);
            if (phase != DrawPhase.Draw)
            {
                return;
            }
            var pos = drawLoc + Extension.HeadCoverDrawOffsetFor(Rotation);
            pos.y = AltitudeLayer.PawnUnused.AltitudeFor();
            HeadCoverGraphic.Draw(pos, Rotation, this);
        }
    }
}
