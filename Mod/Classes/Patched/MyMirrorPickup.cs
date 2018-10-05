using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using Patcher;

namespace Mod
{
    [Patch]
    public class MyMirrorPickup : MirrorPickup
    {
        public MyMirrorPickup(Vector2 position, Vector2 targetPosition)
            : base(position, targetPosition)
        {
            base.Tag(GameTags.PlayerGhostCollider);
        }

        public override void OnPlayerGhostCollide(PlayerGhost ghost)
        {
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems)
            {
                MyPlayerGhost g = (MyPlayerGhost)ghost;
                if (!g.Invisible)
                {
                    base.Level.Layers[g.LayerIndex].Add(new LightFade().Init(this, null));
                    g.Invisible = true;
                    Sounds.pu_invisible.Play(base.X, 1f);
                    base.RemoveSelf();
                }
            }
        }
    }
}
