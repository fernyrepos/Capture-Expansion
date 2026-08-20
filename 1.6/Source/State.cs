using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CaptureExpansion
{
    [StaticConstructorOnStartup]
    public static class State
    {
        public static HashSet<ThingDef> humanRaces = new HashSet<ThingDef>();

        static State()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike && x.IsCorpse is false))
            {
                humanRaces.Add(def);
            }
        }

        public static bool IsRestrained(this Pawn pawn)
        {
            if (pawn.IsCaged()) return false;
            if (pawn.health.hediffSet.HasHediff(DefsOf.CE_RestrainedToBed) || pawn.ownership?.OwnedBed != null && pawn.ownership.OwnedBed.def.HasModExtension<RestraintExtension>() || pawn.IsOnHoldingPlatform) return true;
            return false;
        }

        public static bool IsCaged(this Pawn pawn)
        {
            if (pawn.IsOnHoldingPlatform) return false;
            if (pawn.ownership?.OwnedBed is Building_Cage || pawn.Spawned && pawn.Map.thingGrid.ThingsListAtFast(pawn.Position).Any(t => t is Building_Cage)) return true;
            return false;
        }
    }
}
