using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class ThinkNode_ConditionalRestrained : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            if (pawn.IsPrisoner && PrisonBreakUtility.IsPrisonBreaking(pawn) is false && !(pawn.TryGetComp<CompHoldingPlatformTarget>()?.isEscaping ?? false))
            {
                if (pawn.IsRestrained()) return true;

                if (pawn.Spawned)
                {
                    foreach (var item in pawn.Map.mapPawns.FreeColonistsSpawned)
                    {
                        var p = item;
                        if (p.CurJobDef == DefsOf.CE_RestrainPrisonerToBed && p.CurJob.GetTarget(TargetIndex.A).Thing == pawn && pawn.InBed() && pawn.CurrentBed() == p.CurJob.GetTarget(TargetIndex.B).Thing)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
