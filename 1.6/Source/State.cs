using System.Runtime.CompilerServices;
using Verse;

namespace CaptureExpansion
{
    public static class State
    {
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
