using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    public class CaptureExpansionMod : Mod
    {
        public static Harmony harmony;
        public CaptureExpansionMod(ModContentPack pack) : base(pack)
        {
            harmony = new Harmony("ferny.CaptureExpansion");
            harmony.PatchAll();
        }
    }
}
