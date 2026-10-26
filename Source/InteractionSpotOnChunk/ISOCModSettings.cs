using Verse;

namespace InteractionSpotOnChunk
{
    public class ISOCModSettings : ModSettings
    {
        public bool debugMode = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref debugMode, "debugMode", false);
        }
    }
}
