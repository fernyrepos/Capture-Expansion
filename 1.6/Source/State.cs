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
            if (pawn.guest != null && pawn.guest.IsInteractionEnabled(DefsOf.CE_RestrainToBed)) return true;
            if (pawn.ownership?.OwnedBed != null && pawn.ownership.OwnedBed.def.HasModExtension<RestraintExtension>()) return true;
            if (pawn.IsOnHoldingPlatform) return true;
            return false;
        }

        public static bool IsCaged(this Pawn pawn)
        {
            return pawn.ownership?.OwnedBed != null && pawn.ownership.OwnedBed.def.HasModExtension<CageExtension>();
        }
    }
}
