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
		public override Level Level {
			get;
			set;
		}

		public MyPlayerCorpse (PlayerCorpse.EnemyCorpses enemyCorpse, Vector2 position, Facing facing, int killerIndex) : base (enemyCorpse.ToString (), Allegiance.Neutral, position, facing, -1, killerIndex)
		{
		}

		public override void Added ()
		{
			// The equivalent of calling base.Added() from PlayerCorpse
			for (int i = 0; i < this.Tags.Count; i++) {
				this.Scene.TagEntityInstant (this, this.Tags[i]);
			}
			if (this.Components != null) {
				for (int i = 0; i < this.Components.Count; i++) {
					this.Components[i].EntityAdded ();
				}
			}
			TFGame.Log(new Exception("Up Here!@#"), false);
			Level level = base.Level;
			if (base.Scene is Level) {
				TFGame.Log(new Exception("Needed to do this thing"), false);
				this.Level = (base.Scene as Level);
			}
			TFGame.Log(new Exception("DOwn Here!@#"), false);

			if (this.PlayerIndex != -1) {
				// TFGame.Log(new Exception("Here in the code!@#"), false);
				//
				// TFGame.Log(new Exception("Just doing it!"), false);
				// MyTeamReviver teamReviver = new MyTeamReviver (this, TeamReviver.Modes.TeamDeathmatch);
				// TFGame.Log(new Exception("Created it!"), false);
				// base.Level.Session.CurrentLevel.Layers[teamReviver.LayerIndex].Add(teamReviver, false);

				// if (base.Level.Session.MatchSettings.Mode == Modes.Quest) {
				// 	MyTeamReviver teamReviver = new MyTeamReviver (this, TeamReviver.Modes.Quest);
				// 	base.Level.Layers[teamReviver.LayerIndex].Add(teamReviver, false);
				// } else if (base.Level.Session.MatchSettings.Mode == Modes.DarkWorld) {
				// 	MyTeamReviver teamReviver = new MyTeamReviver (this, TeamReviver.Modes.DarkWorld);
				// 	base.Level.Layers[teamReviver.LayerIndex].Add(teamReviver, false);
				// } else if (base.Level.Session.MatchSettings.Mode == Modes.TeamDeathmatch && base.Level.Session.MatchSettings.Variants.TeamRevive) {
				// 	TFGame.Log(new Exception("Created a team reviver"), false);
				// 	MyTeamReviver teamReviver = new MyTeamReviver (this, TeamReviver.Modes.TeamDeathmatch);
				// 	base.Level.Layers[teamReviver.LayerIndex].Add(teamReviver, false);
				// }
			}
			if (base.Level.Session.MatchSettings.Variants.CorpsesDropArrows [this.PlayerIndex]) {
				this.StartDroppingArrows ();
			}
			if (this.Speed.Y > -1f && base.CollideCheck (GameTags.Solid, this.Position + Vector2.UnitY)) {
				this.Speed.Y = -1f;
			}
			if (this.brambles != null) {
				Color color;
				Color color2;
				Brambles.GetBrambleColors (this.KillerIndex, base.Level.Session.MatchSettings.TeamMode, base.Level.Session.MatchSettings.Teams [this.KillerIndex], out color, out color2);
				FlashingImage[] array = this.brambles;
				for (int i = 0; i < array.Length; i++) {
					FlashingImage flashingImage = array [i];
					flashingImage.StartFlashing (4, new Color[] {
						color,
						color2
					});
				}
			}
			if (this.PlayerIndex != -1 && this.CanExplode) {
				BombPickup.SFXNewest = this;
				Sounds.sfx_bombChestLoop.Play (base.X, 1f);
			}
			if (this.PlayerIndex != -1 && base.Level.Session.MatchSettings.Variants.ReturnAsGhosts [this.PlayerIndex]) {
				this.ghostCoroutine = new Coroutine (this.GhostSpawnSequence ());
			}
			if (this.PlayerIndex == -1) {
				base.Add (new Coroutine (this.DisappearSequence ()));
			}
		}
	}
}
