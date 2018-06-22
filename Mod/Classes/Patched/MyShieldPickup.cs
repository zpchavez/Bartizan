using System;
using Microsoft.Xna.Framework;
using Mod;
using Monocle;
using TowerFall;
using Patcher;

namespace Patched
{
	[Patch]
	public class MyShieldPickup : ShieldPickup
    {
		public MyShieldPickup(Vector2 position, Vector2 targetPosition)
            : base (position, targetPosition)
		{
			base.Tag(GameTags.PlayerGhostCollider);
        }

		public override void OnPlayerGhostCollide(PlayerGhost ghost)
		{
			if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems)
			{
				MyPlayerGhost g = (MyPlayerGhost)ghost;
				if (!g.HasShield)
				{
					base.Level.Layers[g.LayerIndex].Add(new LightFade().Init(this, null));
					base.DoCollectStats(g.PlayerIndex);
					g.HasShield = true;
					base.RemoveSelf();
				}
			}
		}
    }
}
