using Verse;

namespace CaptureExpansion
{
    public class CaptureData : IExposable
    {
        public bool restrainedToBed;

        public void ExposeData()
        {
            Scribe_Values.Look(ref restrainedToBed, "restrainedToBed");
        }
    }
}
