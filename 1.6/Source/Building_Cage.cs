using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class Building_Cage : Building_Bed
    {
        private Graphic topGraphic;
        private GraphicData cachedTopGraphicData;

        private Graphic TopGraphic
        {
            get
            {
                var topGraphicData = GetComp<CompCageTopGraphicVariation>()?.TopGraphicData
                    ?? def.GetModExtension<CageExtension>().topGraphicData;
                if (topGraphicData != cachedTopGraphicData)
                {
                    cachedTopGraphicData = topGraphicData;
                    topGraphic = topGraphicData.Graphic;
                }
                return topGraphic;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ForPrisoners = true;
        }

        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            base.DynamicDrawPhaseAt(phase, drawLoc, flip);
            if (phase == DrawPhase.Draw)
            {
                var pos = drawLoc;
                pos.y = AltitudeLayer.PawnUnused.AltitudeFor();
                TopGraphic.Draw(pos, Rotation, this);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                if (g is Command_Toggle or Command_SetBedOwnerType) continue;
                yield return g;
            }
        }

        public IEnumerable<IntVec3> WanderCells()
        {
            var rect = this.OccupiedRect();
            foreach (var c in rect)
            {
                var isWanderZone = Rotation.AsInt switch
                {
                    0 => c.z >= rect.minZ + rect.Height / 2,
                    1 => c.x >= rect.minX + rect.Width / 2,
                    2 => c.z < rect.minZ + rect.Height / 2,
                    3 => c.x < rect.minX + rect.Width / 2,
                    _ => false
                };
                if (isWanderZone)
                {
                    yield return c;
                }
            }
        }
    }
}
