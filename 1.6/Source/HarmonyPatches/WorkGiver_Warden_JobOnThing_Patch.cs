using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace CaptureExpansion
{
    [HarmonyPatch]
    public static class WorkGiver_Warden_JobOnThing_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(WorkGiver_Warden_Chat), nameof(WorkGiver_Warden_Chat.JobOnThing));
            yield return AccessTools.Method(typeof(WorkGiver_Warden_Convert), nameof(WorkGiver_Warden_Convert.JobOnThing));
            yield return AccessTools.Method(typeof(WorkGiver_Warden_Enslave), nameof(WorkGiver_Warden_Enslave.JobOnThing));
            yield return AccessTools.Method(typeof(WorkGiver_Warden_InterrogateIdentity), nameof(WorkGiver_Warden_InterrogateIdentity.JobOnThing));
        }

        public static void Prefix()
        {
            RestUtility_InBed_Patch.forWardenJob++;
        }

        public static Exception Finalizer(Exception __exception)
        {
            RestUtility_InBed_Patch.forWardenJob--;
            return __exception;
        }
    }
}
