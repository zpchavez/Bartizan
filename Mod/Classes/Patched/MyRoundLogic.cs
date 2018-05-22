using Patcher;
using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System;

namespace Mod
{
	[Patch]
	public class MyRoundLogic : RoundLogic
	{
		protected MyRoundLogic(Session session, bool canHaveMiasma)
			: base(session, canHaveMiasma)
		{
			if (((MyMatchVariants)(session.MatchSettings.Variants)).CalvinFall) {
				session.MatchSettings.Variants.Randomize();
			}
		}

		public override void OnLevelLoadFinish ()
		{
			if (!this.Session.MatchSettings.SoloMode) {
				SaveData.Instance.Stats.RoundsPlayed++;
				SessionStats.RoundsPlayed++;
				((MySession)this.Session).RoundsPlayedThisMatch++;
			}
		}

		public override void OnPlayerDeath (Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
		{
			if (this.Session.MatchSettings.Variants.ReturnAsGhosts[playerIndex]) {
				((MyPlayerCorpse)(corpse)).spawningGhost = true;
				((MyPlayer)(player)).spawningGhost = true;
			}

			if (this.Session.CurrentLevel.KingIntro) {
				this.Session.CurrentLevel.KingIntro.Laugh ();
			}
			if (this.Session.MatchSettings.Variants.GunnStyle) {
				GunnStyle gunnStyle = new GunnStyle (corpse);
				this.Session.CurrentLevel.Layers[gunnStyle.LayerIndex].Add(gunnStyle, false);
			}
			if (
				this.miasma && killerIndex != -1 &&
				this.FFACheckForAllButOneDead () &&
				!((MyMatchVariants)this.Session.MatchSettings.Variants).GottaBustGhosts
			) {
				this.miasma.Dissipate ();
			}
			if (killerIndex != -1 && killerIndex != playerIndex) {
				this.Kills [killerIndex]++;
			}
			if (killerIndex != -1 && !this.Session.CurrentLevel.IsPlayerAlive (killerIndex)) {
				MatchStats[] expr_F8_cp_0 = this.Session.MatchStats;
				expr_F8_cp_0 [killerIndex].KillsWhileDead = expr_F8_cp_0 [killerIndex].KillsWhileDead + 1u;
			}
			if (killerIndex != -1 && killerIndex != playerIndex) {
				this.Session.MatchStats [killerIndex].RegisterFastestKill (this.Time);
			}
			DeathType deathType = DeathType.Normal;
			if (killerIndex == playerIndex) {
				deathType = DeathType.Self;
			} else if (killerIndex != -1 && this.Session.MatchSettings.TeamMode && this.Session.MatchSettings.GetPlayerAllegiance (playerIndex) == this.Session.MatchSettings.GetPlayerAllegiance (killerIndex)) {
				deathType = DeathType.Team;
			}
			if (killerIndex != -1) {
				if (deathType == DeathType.Normal && this.Session.WasWinningAtStartOfRound (playerIndex)) {
					MatchStats[] expr_1C9_cp_0 = this.Session.MatchStats;
					expr_1C9_cp_0 [killerIndex].WinnerKills = expr_1C9_cp_0 [killerIndex].WinnerKills + 1u;
				}
				if (!this.Session.MatchSettings.SoloMode) {
					this.Session.MatchStats [killerIndex].Kills.Add (deathType, cause, TFGame.Characters [playerIndex]);
					SaveData.Instance.Stats.Kills.Add (deathType, cause, TFGame.Characters [killerIndex]);
					SaveData.Instance.Stats.TotalVersusKills++;
					SaveData.Instance.Stats.RegisterVersusKill (killerIndex);
				}
			}
			this.Session.MatchStats [playerIndex].Deaths.Add (deathType, cause, (killerIndex == -1) ? (-1) : TFGame.Characters [killerIndex]);
			SaveData.Instance.Stats.Deaths.Add (deathType, cause, TFGame.Characters [playerIndex]);
			if (!this.Session.MatchSettings.SoloMode) {
				SessionStats.RegisterVersusKill (killerIndex, playerIndex, deathType == DeathType.Team);
			}
		}

		public void OnGhostDeath() {
			if (base.Session.MatchSettings.Mode == Modes.TeamDeathmatch) {
				Allegiance allegiance;
				if (this.TeamCheckForRoundOver(out allegiance)) {
					base.Session.CurrentLevel.Ending = true;
				}
			} else {
				if (this.FFACheckForAllButOneDead()) {
					base.Session.CurrentLevel.Ending = true;
				}
			}
		}

		public new static RoundLogic GetRoundLogic(Session session)
		{
			switch (session.MatchSettings.Mode) {
				case RespawnRoundLogic.Mode:
					return new RespawnRoundLogic(session);
				case MobRoundLogic.Mode:
					return new MobRoundLogic(session);
				default:
					return RoundLogic.GetRoundLogic(session);
			}
		}

		public override bool FFACheckForAllButOneDead ()
		{
			if (((MyMatchVariants)(this.Session.MatchSettings.Variants)).GottaBustGhosts) {
				if (this.Session.CurrentLevel.LivingPlayers == 0) {
					return true;
				}
				// Round not over if ghost spawning
				List<Entity> players = this.Session.CurrentLevel[GameTags.Player];
				for (int i = 0; i < players.Count; i++) {
					MyPlayer player = (MyPlayer) players[i];
					if (player.spawningGhost) {
						return false;
					}
				}
				List<Entity> playerCorpses = this.Session.CurrentLevel[GameTags.Corpse];
				for (int i = 0; i < playerCorpses.Count; i++) {
					MyPlayerCorpse playerCorpse = (MyPlayerCorpse) playerCorpses[i];
					if (playerCorpse.spawningGhost) {
						return false;
					}
				}

				// Round not over if ghosts alive
				List<Entity> playerGhosts = this.Session.CurrentLevel[GameTags.PlayerGhost];
				int livingGhostCount = 0;
				for (int i = 0; i < playerGhosts.Count; i++) {
					MyPlayerGhost ghost = (MyPlayerGhost) playerGhosts[i];
					if (ghost.State != 3) {
						livingGhostCount += 1;
					}
				}
				return livingGhostCount == 0 && this.Session.CurrentLevel.LivingPlayers <= 1;
			} else {
				return this.Session.CurrentLevel.LivingPlayers <= 1;
			}
		}

		public override bool TeamCheckForRoundOver (out Allegiance surviving)
		{
			bool[] array = new bool[2];
			bool gottaBustGhosts = ((MyMatchVariants)(this.Session.MatchSettings.Variants)).GottaBustGhosts;
			List<Entity> players = this.Session.CurrentLevel[GameTags.Player];
			for (int i = 0; i < players.Count; i++) {
				MyPlayer player = (MyPlayer) players[i];
				if (!player.Dead || (gottaBustGhosts && player.spawningGhost)) {
					array [(int)player.Allegiance] = true;
				}
			}

			List<Entity> playerCorpses = this.Session.CurrentLevel[GameTags.Corpse];
			for (int i = 0; i < playerCorpses.Count; i++) {
				MyPlayerCorpse playerCorpse = (MyPlayerCorpse) playerCorpses[i];
				if (playerCorpse.Revived || (gottaBustGhosts && playerCorpse.spawningGhost)) {
					array [(int)playerCorpse.Allegiance] = true;
				}
			}

			if (gottaBustGhosts && players.Count > 1) {
				List<Entity> playerGhosts = this.Session.CurrentLevel[GameTags.PlayerGhost];
				for (int i = 0; i < playerGhosts.Count; i++) {
					PlayerGhost playerGhost = (PlayerGhost) playerGhosts[i];
					if (playerGhost.State != 3) { // Ghost not dead
						array [(int)playerGhost.Allegiance] = true;
					}
				}
			}

			if (array [0] == array [1]) {
				surviving = Allegiance.Neutral;
			} else if (array [0]) {
				surviving = Allegiance.Blue;
			} else {
				surviving = Allegiance.Red;
			}
			return !array [0] || !array [1];
		}

		public override bool CoOpCheckForAllDead ()
		{
			bool result;
			if (this.Session.CurrentLevel.LivingPlayers == 0) {
				List<Entity> playerCorpses = this.Session.CurrentLevel[GameTags.Corpse];
				for (int i = 0; i < playerCorpses.Count; i++) {
					PlayerCorpse playerCorpse = (PlayerCorpse)playerCorpses[i];
					if (playerCorpse.Revived) {
						result = false;
						return result;
					}
				}

				List<Entity> teamRevivers = this.Session.CurrentLevel[GameTags.TeamReviver];
				for (int i = 0; i < teamRevivers.Count; i++) {
					MyTeamReviver teamReviver = (MyTeamReviver)teamRevivers[i];
					if (teamReviver.AutoRevive && !teamReviver.Finished) {
						result = false;
						return result;
					}
				}
				result = true;
			} else {
				result = false;
			}
			return result;
		}
	}
}
