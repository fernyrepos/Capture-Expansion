using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    [StaticConstructorOnStartup]
    public static class WorkGiver_Warden_RedirectToHeldPrisoner_Patch
    {
        static WorkGiver_Warden_RedirectToHeldPrisoner_Patch()
        {
            var prefix = AccessTools.Method(typeof(WorkGiver_Warden_RedirectToHeldPrisoner_Patch), "Prefix");
            foreach (var method in TargetMethods())
            {
                CaptureExpansionMod.harmony.Patch(method, prefix: new HarmonyMethod(prefix));
            }
        }
        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var type in GenTypes.AllSubclasses(typeof(WorkGiver_Warden)))
            {
                if (type.Assembly == typeof(WorkGiver_Warden_RedirectToHeldPrisoner_Patch).Assembly || type == typeof(WorkGiver_Warden_TakeToBed))
                {
                    continue;
                }
                var method = AccessTools.DeclaredMethod(type, nameof(WorkGiver_Scanner.JobOnThing));
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        public static void Prefix(ref Thing __1)
        {
            if (__1 is Building_HoldingPlatform platform && platform.HeldPawn is { RaceProps.Humanlike: true, IsMutant: false } held)
            {
                __1 = held;
            }
        }
    }
}
