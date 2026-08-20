using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [HarmonyPatch]
    public static class ITab_Pawn_Visitor_CanUsePrisonerInteractionMode_Patch
    {
        public static MethodBase TargetMethod()
        {
            foreach (var type in typeof(ITab_Pawn_Visitor).GetNestedTypes(AccessTools.all))
            {
                var methods = type.GetMethods(AccessTools.all);
                foreach (var method in methods)
                {
                    if (method.Name.Contains("CanUsePrisonerInteractionMode"))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        public static void Postfix(Pawn pawn, PrisonerInteractionModeDef mode, ref bool __result)
        {
            if (__result && mode == DefsOf.CE_RestrainToBed && (pawn.IsCaged() || pawn.IsOnHoldingPlatform))
            {
                __result = false;
            }
        }
    }
}
