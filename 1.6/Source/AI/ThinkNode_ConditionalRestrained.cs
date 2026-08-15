using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureExpansion
{
    public class ThinkNode_ConditionalRestrained : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            return pawn.IsPrisoner && PrisonBreakUtility.IsPrisonBreaking(pawn) is false && State.IsRestrained(pawn);
        }
    }
}
