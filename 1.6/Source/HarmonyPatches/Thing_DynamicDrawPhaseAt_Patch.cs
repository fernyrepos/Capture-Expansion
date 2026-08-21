using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    [HotSwappable]
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DynamicDrawPhaseAt), new Type[] {
        typeof(DrawPhase),
        typeof(Vector3),
        typeof(bool)
    })]
    public static class Thing_DynamicDrawPhaseAt_Patch
    {
        public static void Postfix(Pawn __instance, DrawPhase phase)
        {
            if (phase != DrawPhase.Draw) return;
            var bed = __instance.CurrentBed();
            if (bed != null && bed is not Building_Cage && __instance.IsRestrained())
            {
                var cornerX = (bed.def.size.x / 2f) - 0.05f;
                var cornerZ = (bed.def.size.z / 2f) - 0.05f;
                var bedCenter = bed.DrawPos;
                var bodyAngle = __instance.Drawer.renderer.BodyAngle(PawnRenderFlags.None);
                var bodyQuat = Quaternion.AngleAxis(bodyAngle, Vector3.up);
                var scaleFactor = __instance.ageTracker?.CurLifeStage?.attachPointScaleFactor ?? 1f;

                var off0 = GetAttachPointOffset(__instance, AttachPointType.PlatformRestraint0) * scaleFactor;
                var off1 = GetAttachPointOffset(__instance, AttachPointType.PlatformRestraint1) * scaleFactor;
                var off2 = GetAttachPointOffset(__instance, AttachPointType.PlatformRestraint2) * scaleFactor;
                var off3 = GetAttachPointOffset(__instance, AttachPointType.PlatformRestraint3) * scaleFactor;

                var cornerTL = bedCenter + bodyQuat * new Vector3(-cornerX, 0f, cornerZ);
                var cornerTR = bedCenter + bodyQuat * new Vector3(cornerX, 0f, cornerZ);
                var cornerBR = bedCenter + bodyQuat * new Vector3(cornerX, 0f, -cornerZ);
                var cornerBL = bedCenter + bodyQuat * new Vector3(-cornerX, 0f, -cornerZ);

                var animOffset = Vector3.zero;
                var animRot = Quaternion.identity;
                var parms = new PawnDrawParms
                {
                    pawn = __instance,
                    facing = Rot4.South,
                    rotDrawMode = RotDrawMode.Fresh,
                    posture = __instance.GetPosture(),
                    flags = PawnRenderFlags.Headgear | PawnRenderFlags.Clothes,
                    tint = Color.white
                };
                __instance.Drawer.renderer.renderTree.GetRootTPRS(parms, out animOffset, out _, out animRot, out _);

                var point0 = bedCenter + bodyQuat * (animOffset + animRot * off0);
                var point1 = bedCenter + bodyQuat * (animOffset + animRot * off1);
                var point2 = bedCenter + bodyQuat * (animOffset + animRot * off2);
                var point3 = bedCenter + bodyQuat * (animOffset + animRot * off3);

                DrawRopeSegment(cornerTL, point0, bed);
                DrawRopeSegment(cornerTR, point1, bed);
                DrawRopeSegment(cornerBR, point2, bed);
                DrawRopeSegment(cornerBL, point3, bed);
            }
        }

        private static Vector3 GetAttachPointOffset(Pawn pawn, AttachPointType type)
        {
            var list = pawn.story?.bodyType?.attachPoints;
            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].type == type)
                    {
                        return list[i].offset;
                    }
                }
            }
            return type switch
            {
                AttachPointType.PlatformRestraint0 => new Vector3(-0.25f, 0f, 0.1f),
                AttachPointType.PlatformRestraint1 => new Vector3(0.25f, 0f, 0.1f),
                AttachPointType.PlatformRestraint2 => new Vector3(0.2f, 0f, -0.45f),
                AttachPointType.PlatformRestraint3 => new Vector3(-0.2f, 0f, -0.45f),
                _ => Vector3.zero
            };
        }

        private static void DrawRopeSegment(Vector3 from, Vector3 to, Thing thing)
        {
            var dist = Vector3.Distance(from.WithY(0f), to.WithY(0f));
            if (dist < 0.05f) return;

            var altitude = AltitudeLayer.BuildingOnTop.AltitudeFor() + 0.05f;
            var center = (from + to) / 2f;
            center.y = altitude;

            var angle = (to.WithY(0f) - from.WithY(0f)).normalized.ToAngleFlat();

            var ropeSize = new Vector2(dist, 1f);
            var ropeGraphic = (GraphicDatabase.Get<Graphic_Tiling>("Things/Building/HoldingPlatform/HoldingPlatform_EntityRope", ShaderTypeDefOf.Cutout.Shader, ropeSize, Color.white) as Graphic_Tiling).WithTiling(ropeSize);
            ropeGraphic.Draw(center, Rot4.North, thing, angle + 180f);

            var baseFastener = GraphicDatabase.Get<Graphic_Single>("Things/Building/HoldingPlatform/HoldingPlatform_ChainFastener", ShaderTypeDefOf.Cutout.Shader, new Vector2(0.5f, 0.5f), Color.white);
            var targetFastener = GraphicDatabase.Get<Graphic_Single>("Things/Building/HoldingPlatform/HoldingPlatform_ChainFastener_StrongOutline", ShaderTypeDefOf.Cutout.Shader, new Vector2(0.5f, 0.5f), Color.white);

            baseFastener.Draw(from.WithY(altitude + 0.01f), Rot4.North, thing, angle + 90f);
            targetFastener.Draw(to.WithY(altitude + 0.01f), Rot4.North, thing, angle + 90f);
        }
    }
}
