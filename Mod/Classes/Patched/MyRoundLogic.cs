using Patcher;
using TowerFall;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Mod
{
  [Patch]
	public class MyRoundLogic : RoundLogic
	{
		protected MyRoundLogic(Session session, bool canHaveMiasma)
			: base(session, canHaveMiasma)
		{
		}

		public override void OnPlayerDeath (Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
		{
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
	}
}
