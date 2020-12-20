using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;
using System;
using Patcher;

namespace Mod
{
    [Patch]
    public class MyOrbPickup : OrbPickup
    {
        public MyOrbPickup(Vector2 position, Vector2 targetPosition, OrbTypes orbType) : base(position, targetPosition, orbType)
        {
            base.Tag(GameTags.PlayerGhostCollider);
        }

        // Copied from Monocle's Calc and removed the generic typing so it works with the patcher
        public ParticleType GiveMe (int index, ParticleType a, ParticleType b, ParticleType c, ParticleType d)
        {
            switch (index) {
            default:
                throw new Exception ("Index was out of range!");
            case 0:
                return a;
            case 1:
                return b;
            case 2:
                return c;
            case 3:
                return d;
            }
        }

        public override void OnPlayerGhostCollide(PlayerGhost ghost)
        {
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems)
            {
                base.Level.Layers[ghost.LayerIndex].Add(new LightFade().Init(this, null));
                base.DoCollectStats(ghost.PlayerIndex);
                switch (this.orbType)
                {
                    case OrbTypes.Dark:
                        Sounds.pu_darkOrbCollect.Play(base.X, 1f);
                        base.Level.OrbLogic.DoDarkOrb();
                        base.Level.Particles.Emit(Particles.DarkOrbCollect, 12, base.Position, Vector2.One * 4f);
                        break;
                    case OrbTypes.Time:
                        Sounds.pu_darkOrbCollect.Play(base.X, 1f);
                        base.Level.OrbLogic.DoTimeOrb(false);
                        base.Level.Particles.Emit(Particles.TimeOrbCollect, 12, base.Position, Vector2.One * 4f);
                        break;
                    case OrbTypes.Lava:
                        Sounds.pu_lava.Play(base.X, 1f);
                        base.Level.OrbLogic.DoLavaOrb(ghost.PlayerIndex);
                        base.Level.Particles.Emit(Particles.LavaOrbCollect, 12, base.Position, Vector2.One * 4f);
                        break;
                    case OrbTypes.Space:
                        Sounds.pu_spaceOrb.Stop(true);
                        Sounds.pu_spaceOrb.Play(210f, 1f);
                        base.Level.OrbLogic.DoSpaceOrb();
                        base.Level.Particles.Emit(Particles.SpaceOrbCollect, 12, base.Position, Vector2.One * 4f);
                        break;
                    case OrbTypes.Chaos:
                        Sounds.pu_darkOrbCollect.Play (base.X, 1f);
                        base.Level.OrbLogic.DoDarkOrb ();
                        base.Level.OrbLogic.DoTimeOrb (true);
                        base.Level.OrbLogic.DoLavaOrb (ghost.PlayerIndex);
                        if (!SaveData.Instance.Options.RemoveScrollEffects) {
                            base.Level.OrbLogic.DoSpaceOrbDelayed ();
                        }
                        ParticleType type = this.GiveMe (this.chaosIndex, Particles.DarkOrbCollect, Particles.TimeOrbCollect, Particles.LavaOrbCollect, Particles.SpaceOrbCollect);
                        base.Level.Particles.Emit (type, 12, base.Position, Vector2.One * 4f);
                        break;
                }
                base.RemoveSelf();
            }
        }
    }
}
