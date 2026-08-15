using HarmonyLib;
using Verse;

namespace CaptureExpansion
{
    public class CaptureExpansionMod : Mod
    {
        public CaptureExpansionMod(ModContentPack pack) : base(pack)
        {
            new Harmony("ferny.CaptureExpansion").PatchAll();
        }
    }
}
