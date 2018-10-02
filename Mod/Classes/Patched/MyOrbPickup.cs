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

        public override void OnPlayerGhostCollide(PlayerGhost ghost)
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
            }
            base.RemoveSelf();
        }
    }
}
