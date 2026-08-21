using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CaptureExpansion
{
    public class Hediff_RestrainedToBed : Hediff
    {
        public override void Tick()
        {
            base.Tick();
            if (pawn.IsHashIntervalTick(60) && pawn.InBed() is false)
            {
                pawn.health.RemoveHediff(this);
                return;
            }

            if (pawn.Awake() is false || pawn.Downed)
            {
                if (pawn.Drawer.renderer.CurAnimation != null)
                {
                    pawn.Drawer.renderer.SetAnimation(null);
                }
                return;
            }

            if (pawn.Spawned && Rand.MTBEventOccurs(100f, 1f, 1f))
            {
                UpdateAnimation();
            }
        }

        private void UpdateAnimation()
        {
            var soundDef = DefsOf.EntityChainLow;
            var animationDef = AnimationDefOf.HoldingPlatformWiggleLight;
            if (TryGetFirstColonistDirection(out var direction))
            {
                if (Rand.Chance(0.25f))
                {
                    var vector = direction.normalized.Cardinalize();
                    if (vector == Vector2.up)
                    {
                        animationDef = AnimationDefOf.HoldingPlatformLungeUp;
                    }
                    else if (vector == Vector2.right)
                    {
                        animationDef = AnimationDefOf.HoldingPlatformLungeRight;
                    }
                    else if (vector == Vector2.left)
                    {
                        animationDef = AnimationDefOf.HoldingPlatformLungeLeft;
                    }
                    else if (vector == Vector2.down)
                    {
                        animationDef = AnimationDefOf.HoldingPlatformLungeDown;
                    }
                    soundDef = DefsOf.EntityChainHigh;
                }
                else
                {
                    animationDef = AnimationDefOf.HoldingPlatformWiggleIntense;
                }
            }

            if (pawn.Drawer.renderer.CurAnimation != animationDef)
            {
                soundDef.PlayOneShot(pawn);
                pawn.Drawer.renderer.SetAnimation(animationDef);
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (pawn.Drawer.renderer.CurAnimation != null)
            {
                pawn.Drawer.renderer.SetAnimation(null);
            }
        }

        private bool TryGetFirstColonistDirection(out Vector2 direction)
        {
            foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, 4f, useCenter: false))
            {
                if (item is Pawn { IsColonist: true } col && col.Downed is false)
                {
                    direction = col.Position.ToVector2() - pawn.Position.ToVector2();
                    return true;
                }
            }
            direction = Vector2.zero;
            return false;
        }
    }
}
