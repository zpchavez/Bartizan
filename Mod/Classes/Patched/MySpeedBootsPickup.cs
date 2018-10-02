using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;
using Patcher;

namespace Mod
{
    [Patch]
    public class MySpeedBootsPickup : SpeedBootsPickup
    {
        public MySpeedBootsPickup(Vector2 position, Vector2 targetPosition)
            : base(position, targetPosition)
        {
            base.Tag(GameTags.PlayerGhostCollider);
        }

        public override void OnPlayerGhostCollide(PlayerGhost ghost)
        {
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems)
            {
                MyPlayerGhost g = (MyPlayerGhost)ghost;
                if (!g.HasSpeedBoots)
                {
                    Sounds.pu_speedBoots.Play(base.X, 1f);
                    g.HasSpeedBoots = true;
                    base.Level.Layers[g.LayerIndex].Add(new LightFade().Init(this, null));
                    for (int i = 0; i < 30; i++)
                    {
                        base.Level.Particles.Emit(Particles.SpeedBootsPickup, 1, base.Position, Vector2.One * 3f, 6.28318548f * (float)i / 30f);
                    }
                    base.Level.Particles.Emit(Particles.SpeedBootsPickup2, 18, base.Position, Vector2.One * 4f);
                    base.DoCollectStats(g.PlayerIndex);
                    base.RemoveSelf();
                }
            }
        }
    }
}
