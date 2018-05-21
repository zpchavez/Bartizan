using Patcher;
using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System;

namespace Mod
{
	[Patch]
	public class MyPlayerCorpse : PlayerCorpse
	{
		public bool reviverAdded;

		public bool spawningGhost = true;

		public MyPlayerCorpse (PlayerCorpse.EnemyCorpses enemyCorpse, Vector2 position, Facing facing, int killerIndex) : base (enemyCorpse.ToString (), Allegiance.Neutral, position, facing, -1, killerIndex)
		{
		}

		public override void Update ()
		{
			base.Update();

			if (this.reviverAdded == true) {
				this.reviverAdded = false;
				List<Entity> teamRevivers = base.Level[GameTags.TeamReviver];
				if (teamRevivers.Count > 0) {
					TeamReviver teamReviver = (TeamReviver)(teamRevivers[teamRevivers.Count - 1]);
					base.Level.Layers[teamReviver.LayerIndex].Remove(teamReviver);
					MyTeamReviver myTeamReviver = new MyTeamReviver (
						this,
						TeamReviver.Modes.TeamDeathmatch,
						((MyMatchVariants)(base.Level.Session.MatchSettings.Variants)).GhostRevives
					);
					base.Level.Layers[myTeamReviver.LayerIndex].Add(myTeamReviver, false);
				}
			}
		}

		public override void Added ()
		{
			base.Added();

			if (base.Level.Session.MatchSettings.Variants.ReturnAsGhosts) {
				this.spawningGhost = true;
			}

			if (this.PlayerIndex != -1) {
				if (base.Level.Session.MatchSettings.Mode == Modes.TeamDeathmatch && base.Level.Session.MatchSettings.Variants.TeamRevive) {
					reviverAdded = true;
				}
			}
		}
	}
}
