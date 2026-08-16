using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private static readonly ConditionalWeakTable<Pawn, CaptureData> pawnData = new();

        public static CaptureData GetData(this Pawn pawn)
        {
            if (pawnData.TryGetValue(pawn, out var data) is false)
            {
                data = new CaptureData();
                pawnData.Add(pawn, data);
            }
            return data;
        }

        public static bool TryGetData(this Pawn pawn, out CaptureData data)
        {
            return pawnData.TryGetValue(pawn, out data);
        }

        public static bool IsRestrained(this Pawn pawn)
        {
            if (TryGetData(pawn, out var data) && data.restrainedToBed) return true;
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
