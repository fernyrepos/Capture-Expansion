using RimWorld;
using Verse;

namespace CaptureExpansion
{
    public class PlaceWorker_WallCot : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            var dir = rot.FacingCell.RotatedBy(Rot4.East);
            var p1 = loc - dir;
            var p2 = loc + dir;

            if (IsWall(p1, map) is false || IsWall(p2, map) is false)
            {
                return "CE_RequiresWallsFirstAndThird".Translate();
            }
            return true;
        }

        private bool IsWall(IntVec3 c, Map map)
        {
            if (c.InBounds(map) is false) return false;
            var edifice = c.GetEdifice(map);
            return edifice != null && edifice.def.building != null && edifice.def.building.isWall;
        }
    }
}
