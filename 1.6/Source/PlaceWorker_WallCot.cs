using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HotSwappable]
    public class PlaceWorker_WallCot : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            var rect = GenAdj.OccupiedRect(loc, rot, checkingDef.Size);
            foreach (var cell in rect.GetEdgeCells(rot))
            {
                if (IsWall(cell + rot.FacingCell.RotatedBy(RotationDirection.Counterclockwise), map) is false)
                {
                    return "CE_RequiresWalls".Translate();
                }
            }
            return true;
        }

        private static bool IsWall(IntVec3 c, Map map)
        {
            if (c.InBounds(map) is false) return false;
            var edifice = c.GetEdifice(map);
            return edifice != null && edifice.def.building != null && edifice.def.building.isWall;
        }
    }
}
