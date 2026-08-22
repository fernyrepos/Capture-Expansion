using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    public class Building_RestraintPlatform : Building_HoldingPlatform, IThingHolderWithDrawnPawn
    {
        private RestraintExtension cachedRestraintExt;

        public RestraintExtension RestraintExtension => cachedRestraintExt ??= def.GetModExtension<RestraintExtension>();

        public new PawnPosture HeldPawnPosture => RestraintExtension.heldPawnPosture;
        public new float HeldPawnBodyAngle => (HeldPawnPosture == PawnPosture.Standing ? 0f : Rotation.IsHorizontal && RestraintExtension.usePlatformAngle is false ? 0f : RestraintExtension.faceRotation ? Rotation.AsAngle : Rotation.Opposite.AsAngle) + RestraintExtension.AngleOffsetFor(Rotation);
        public new Rot4 HeldPawnRotation
        {
            get
            {
                if (Rotation.IsHorizontal && RestraintExtension.usePlatformAngle == false)
                {
                    return Rotation.Opposite;
                }
                if (RestraintExtension.fixedRotation != null)
                {
                    return RestraintExtension.fixedRotation.Value;
                }
                if (HeldPawnPosture == PawnPosture.Standing)
                {
                    return RestraintExtension.faceRotation ? Rotation : Rotation.Opposite;
                }
                return Rot4.South;
            }
        }
        public new float HeldPawnDrawPos_Y => DrawPos.y + PawnDrawOffset.y;
        public new Vector3 PawnDrawOffset => RestraintExtension.PawnDrawOffsetFor(Rotation);

        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            if (phase == DrawPhase.Draw)
            {
                DrawAt(drawLoc, flip);
            }
            var heldPawn = HeldPawn;
            if (heldPawn != null)
            {
                var pawnPos = DrawPos + PawnDrawOffset;
                heldPawn.Drawer.renderer.DynamicDrawPhaseAt(phase, pawnPos, HeldPawnRotation, neverAimWeapon: true);
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (def.drawerType == DrawerType.RealtimeOnly || Spawned is false)
            {
                Graphic.Draw(drawLoc, flip ? Rotation.Opposite : Rotation, this);
            }
            SilhouetteUtility.DrawGraphicSilhouette(this, drawLoc);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.def.inspectorTabsResolved ??= new List<InspectTabBase>();
        }
    }
}
