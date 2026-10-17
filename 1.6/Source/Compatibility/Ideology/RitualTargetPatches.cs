using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion.Compatibility.Ideology
{
    [HarmonyPatch]
    internal static class RitualTargetValidationPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(RitualObligationTargetWorker_AnyRitualSpotOrAltar), "CanUseTargetInternal");
            yield return AccessTools.Method(typeof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar), "CanUseTargetInternal");
        }

        private static void Postfix(RitualObligationTargetFilter __instance, TargetInfo target,
            ref RitualTargetUseReport __result)
        {
            Building_RestraintPlatform platform = RitualExecutionUtility.Device(target.Thing);
            if (platform == null || !RitualExecutionUtility.Supported(__instance.parent))
                return;
            string issue = RitualExecutionUtility.StartIssue(__instance.parent, platform);
            __result = issue == null ? (RitualTargetUseReport)true : issue;
        }
    }

    [HarmonyPatch]
    internal static class RitualTargetEnumerationPatch
    {
        private static bool Prepare() => ModsConfig.IdeologyActive;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(RitualObligationTargetWorker_AnyRitualSpotOrAltar), "GetTargets");
            yield return AccessTools.Method(typeof(RitualObligationTargetWorker_AnyRitualSpotIdeogramOrAltar), "GetTargets");
        }

        private static void Postfix(RitualObligationTargetFilter __instance, Map map,
            ref IEnumerable<TargetInfo> __result)
        {
            if (map != null && RitualExecutionUtility.Supported(__instance.parent))
                __result = IncludeDevices(__result, __instance.parent, map);
        }

        private static IEnumerable<TargetInfo> IncludeDevices(IEnumerable<TargetInfo> originals,
            Precept_Ritual ritual, Map map)
        {
            foreach (TargetInfo target in originals)
                yield return target;
            foreach (TargetInfo target in TargetsForDef("CE_Guillotine", ritual, map))
                yield return target;
            foreach (TargetInfo target in TargetsForDef("CE_ElectricChair", ritual, map))
                yield return target;
        }

        private static IEnumerable<TargetInfo> TargetsForDef(string name, Precept_Ritual ritual, Map map)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(name);
            if (def == null)
                yield break;
            foreach (Thing thing in map.listerThings.ThingsOfDef(def))
            {
                Building_RestraintPlatform platform = RitualExecutionUtility.Device(thing);
                if (platform != null && RitualExecutionUtility.StartIssue(ritual, platform) == null)
                    yield return platform;
            }
        }
    }
}
