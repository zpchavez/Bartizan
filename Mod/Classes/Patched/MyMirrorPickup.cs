using System;
using Microsoft.Xna.Framework;
using Mod;
using Monocle;
using TowerFall;
using Patcher;

namespace Patched
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
    				TFGame.Log(new Exception("Make Ghost invisible"), false);
    				base.Level.Layers[g.LayerIndex].Add(new LightFade().Init(this, null));
    				g.Invisible = true;
    				Sounds.pu_invisible.Play(base.X, 1f);
    				base.RemoveSelf();
    			}
    		}
        }
    }
}
