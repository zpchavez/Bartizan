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
			//TFGame.Log(new Exception("ghost grabbed shield"), false);
            MyPlayerGhost g = (MyPlayerGhost)ghost;
			TFGame.Log(new Exception(g.HasShield.ToString()), false);
			if (!g.HasShield)
            {
                //base.Level.Add(Cache.Create<LightFade>().Init(this, null));
				base.DoCollectStats(ghost.PlayerIndex);
				g.HasShield = true;
                //base.RemoveSelf();
            }
			base.RemoveSelf();
		}
    }
}
