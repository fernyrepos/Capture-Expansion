using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class Building_Cage : Building_Bed, IPathFindCostProvider
    {
        private Graphic topGraphic;
        private GraphicData cachedTopGraphicData;
        private List<IntVec3> wanderCells;

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

        public List<IntVec3> WanderCells => wanderCells ??= BuildWanderCells().ToList();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            wanderCells = null;
            base.SpawnSetup(map, respawningAfterLoad);
            ForPrisoners = true;
        }

        public ushort PathFindCostFor(Pawn pawn)
        {
            if (pawn.ownership.OwnedBed == this || CurOccupants.Contains(pawn))
                return 0;
            if (pawn.carryTracker.CarriedThing is Pawn c && (c.ownership.OwnedBed == this || CurOccupants.Contains(c)))
                return 0;
            if (pawn.CurJob != null && (pawn.CurJob.AnyTargetIs(this) || this.OccupiedRect().Contains(pawn.CurJob.targetA.Cell) || this.OccupiedRect().Contains(pawn.CurJob.targetB.Cell)))
                return 0;
            return 800;
        }

        public CellRect GetOccupiedRect() => this.OccupiedRect();

        public override ushort PathWalkCostFor(Pawn p)
        {
            if (p.ownership.OwnedBed == this || CurOccupants.Contains(p))
                return 0;
            if (p.carryTracker.CarriedThing is Pawn c && (c.ownership.OwnedBed == this || CurOccupants.Contains(c)))
                return 0;
            if (p.CurJob != null && (p.CurJob.AnyTargetIs(this) || this.OccupiedRect().Contains(p.CurJob.targetA.Cell) || this.OccupiedRect().Contains(p.CurJob.targetB.Cell)))
                return 0;
            return 30;
        }

        public override void DrawGUIOverlay()
        {
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

        private IEnumerable<IntVec3> BuildWanderCells()
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
