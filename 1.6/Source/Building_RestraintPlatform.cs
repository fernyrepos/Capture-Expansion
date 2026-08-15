using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class Building_RestraintPlatform : Building_HoldingPlatform
    {
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (def.drawerType == DrawerType.RealtimeOnly || Spawned is false)
            {
                Graphic.Draw(drawLoc, flip ? Rotation.Opposite : Rotation, this);
            }
            SilhouetteUtility.DrawGraphicSilhouette(this, drawLoc);
        }
    }
}
