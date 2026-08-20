using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureExpansion
{
    public class CageExtension : DefModExtension
    {
        public GraphicData topGraphicData;
    }
    public class RestraintExtension : DefModExtension
    {
        public PawnPosture heldPawnPosture = PawnPosture.LayingOnGroundFaceUp;
        public bool faceRotation;
        public bool usePlatformAngle = true;
        public bool crawlingHeadNorth;
        public Vector3? pawnDrawOffsetNorth;
        public Vector3? pawnDrawOffsetEast;
        public Vector3? pawnDrawOffsetSouth;
        public Vector3? pawnDrawOffsetWest;
        public Vector3? headDrawOffsetNorth;
        public Vector3? headDrawOffsetEast;
        public Vector3? headDrawOffsetSouth;
        public Vector3? headDrawOffsetWest;
        public Vector3? bodyDrawOffsetNorth;
        public Vector3? bodyDrawOffsetEast;
        public Vector3? bodyDrawOffsetSouth;
        public Vector3? bodyDrawOffsetWest;
        public float? angleOffsetNorth;
        public float? angleOffsetEast;
        public float? angleOffsetSouth;
        public float? angleOffsetWest;
        public Rot4? fixedRotation;

        public Vector3 PawnDrawOffsetFor(Rot4 rot)
        {
            if (rot == Rot4.North && pawnDrawOffsetNorth.HasValue) return pawnDrawOffsetNorth.Value;
            if (rot == Rot4.East && pawnDrawOffsetEast.HasValue) return pawnDrawOffsetEast.Value;
            if (rot == Rot4.South && pawnDrawOffsetSouth.HasValue) return pawnDrawOffsetSouth.Value;
            if (rot == Rot4.West && pawnDrawOffsetWest.HasValue) return pawnDrawOffsetWest.Value;
            return Vector3.zero;
        }

        public float AngleOffsetFor(Rot4 rot)
        {
            if (rot == Rot4.North && angleOffsetNorth.HasValue) return angleOffsetNorth.Value;
            if (rot == Rot4.East && angleOffsetEast.HasValue) return angleOffsetEast.Value;
            if (rot == Rot4.South && angleOffsetSouth.HasValue) return angleOffsetSouth.Value;
            if (rot == Rot4.West && angleOffsetWest.HasValue) return angleOffsetWest.Value;
            return 0f;
        }

        public Vector3 HeadDrawOffsetFor(Rot4 rot)
        {
            if (rot == Rot4.North && headDrawOffsetNorth.HasValue) return headDrawOffsetNorth.Value;
            if (rot == Rot4.East && headDrawOffsetEast.HasValue) return headDrawOffsetEast.Value;
            if (rot == Rot4.South && headDrawOffsetSouth.HasValue) return headDrawOffsetSouth.Value;
            if (rot == Rot4.West && headDrawOffsetWest.HasValue) return headDrawOffsetWest.Value;
            return Vector3.zero;
        }

        public Vector3 BodyDrawOffsetFor(Rot4 rot)
        {
            if (rot == Rot4.North && bodyDrawOffsetNorth.HasValue) return bodyDrawOffsetNorth.Value;
            if (rot == Rot4.East && bodyDrawOffsetEast.HasValue) return bodyDrawOffsetEast.Value;
            if (rot == Rot4.South && bodyDrawOffsetSouth.HasValue) return bodyDrawOffsetSouth.Value;
            if (rot == Rot4.West && bodyDrawOffsetWest.HasValue) return bodyDrawOffsetWest.Value;
            return Vector3.zero;
        }
    }

    public class CompProperties_CageTopGraphicVariation : CompProperties
    {
        public List<GraphicData> topGraphics;
        public List<string> optionalNames;

        public CompProperties_CageTopGraphicVariation()
        {
            compClass = typeof(CompCageTopGraphicVariation);
        }
    }

    public class CompCageTopGraphicVariation : ThingComp
    {
        private int graphicIndex = -1;

        public CompProperties_CageTopGraphicVariation Props => (CompProperties_CageTopGraphicVariation)props;

        public GraphicData TopGraphicData
        {
            get
            {
                if (Props.topGraphics.NullOrEmpty())
                {
                    return parent.def.GetModExtension<CageExtension>().topGraphicData;
                }
                EnsureGraphicIndex();
                return Props.topGraphics[graphicIndex];
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                graphicIndex = 0;
            }
            EnsureGraphicIndex();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref graphicIndex, "graphicIndex", -1);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (Props.topGraphics.NullOrEmpty() || Props.topGraphics.Count < 2)
            {
                yield break;
            }

            EnsureGraphicIndex();
            yield return new Command_Action
            {
                defaultLabel = "CE_ChangeCageTopGraphic".Translate(),
                defaultDesc = "CE_ChangeCageTopGraphicDesc".Translate(),
                icon = TopGraphicData.Graphic.MatSingle.mainTexture as UnityEngine.Texture2D,
                iconDrawScale = 1.3f,
                iconOffset = new Vector2(0f, 0.08f),
                action = () =>
                {
                    graphicIndex = (graphicIndex + 1) % Props.topGraphics.Count;
                }
            };
        }

        private void EnsureGraphicIndex()
        {
            if (Props.topGraphics.NullOrEmpty())
            {
                graphicIndex = 0;
            }
            else if (graphicIndex < 0 || graphicIndex >= Props.topGraphics.Count)
            {
                graphicIndex = 0;
            }
        }
    }
}
