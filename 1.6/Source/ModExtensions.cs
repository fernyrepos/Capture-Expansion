using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CaptureExpansion
{
    public class CageExtension : DefModExtension
    {
        public GraphicData topGraphicData;
    }
    public class RestraintExtension : DefModExtension { }

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
                graphicIndex = Props.topGraphics.NullOrEmpty() ? 0 : Rand.Range(0, Props.topGraphics.Count);
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
