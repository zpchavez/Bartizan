using Patcher;
using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System;

namespace Mod
{
	[Patch]
	public class MyRoundLogic : RoundLogic, GhostDeathInterface
	{
		public static bool myLogic;
		public static RoundLogic myRoundLogic;
		protected MyRoundLogic(Session session, bool canHaveMiasma)
			: base(session, canHaveMiasma)
		{
			if (((MyMatchVariants)(session.MatchSettings.Variants)).CalvinFall)
			{
				session.MatchSettings.Variants.Randomize();
			}
		}

        public static RoundLogic GetMyRoundLogic(MySession session)
        {
			myLogic = false;
            switch (session.MatchSettings.Mode)
			{
				case Modes.Trials:
                    return new TrialsRoundLogic(session);
                case Modes.Quest:
                    return new QuestRoundLogic(session);
                case Modes.DarkWorld:
                    return new DarkWorldRoundLogic(session);
                case Modes.LastManStanding:
                    return new LastManStandingRoundLogic(session);
                case Modes.HeadHunters:
                    return new HeadhuntersRoundLogic(session);
                case Modes.TeamDeathmatch:
                    return new MyTeamDeathmatchRoundLogic(session);
                case Modes.Warlord:
                    return new WarlordRoundLogic(session);
                case Modes.LevelTest:
                    return new LevelTestRoundLogic(session);
                case RespawnRoundLogic.Mode:
                    return new RespawnRoundLogic(session);
                case MobRoundLogic.Mode:
                    return new MobRoundLogic(session);
                default:
					throw new Exception("No defined Round Logic for that mode!");
            }
        }

		public override void OnLevelLoadFinish()
		{
			if (!this.Session.MatchSettings.SoloMode)
			{
				SaveData.Instance.Stats.RoundsPlayed++;
				SessionStats.RoundsPlayed++;
				((MySession)this.Session).RoundsPlayedThisMatch++;
			}
		}

		public override bool FFACheckForAllButOneDead()
        {
			if (((MyMatchVariants)(this.Session.MatchSettings.Variants)).GottaBustGhosts)
			{
                if (this.Session.CurrentLevel.LivingPlayers == 0)
				{
					return true;
				}

				// Round not over if ghost spawning
				List<Entity> players = this.Session.CurrentLevel[GameTags.Player];
				for (int i = 0; i < players.Count; i++)
				{
					MyPlayer player = (MyPlayer)players[i];
					if (player.spawningGhost)
					{
						return false;
					}
				}

				List<Entity> playerCorpses = this.Session.CurrentLevel[GameTags.Corpse];
				for (int i = 0; i < playerCorpses.Count; i++)
				{
					MyPlayerCorpse playerCorpse = (MyPlayerCorpse)playerCorpses[i];
					if (playerCorpse.spawningGhost)
					{
						return false;
					}
				}

                // Round not over if ghosts alive
                List<Entity> playerGhosts = this.Session.CurrentLevel[GameTags.PlayerGhost];
                int livingGhostCount = 0;
                for (int i = 0; i < playerGhosts.Count; i++)
				{
                    MyPlayerGhost ghost = (MyPlayerGhost)playerGhosts[i];
                    if (ghost.State != 3)
					{
                        livingGhostCount += 1;
                    }
                }
                return livingGhostCount == 0 && this.Session.CurrentLevel.LivingPlayers <= 1;
            }
			else
			{
                return this.Session.CurrentLevel.LivingPlayers <= 1;
            }
        }

        public override bool TeamCheckForRoundOver(out Allegiance surviving)
        {
			TFGame.Log(new Exception(), false);
            if (this.Session.CurrentLevel.LivingPlayers == 0)
			{
				surviving = Allegiance.Neutral;
                return true;
            }
            bool[] array = new bool[2];
            TFGame.Log(new Exception("1 red: " + array[0] + ", blue: " + array[1]), false);
            bool gottaBustGhosts = ((MyMatchVariants)(this.Session.MatchSettings.Variants)).GottaBustGhosts;
            List<Entity> players = this.Session.CurrentLevel[GameTags.Player];
            for (int i = 0; i < players.Count; i++)
			{
                MyPlayer player = (MyPlayer)players[i];
                if (!player.Dead || (gottaBustGhosts && player.spawningGhost))
				{
					array[(int)player.Allegiance] = true;
                }
            }

            List<Entity> playerCorpses = this.Session.CurrentLevel[GameTags.Corpse];
            for (int i = 0; i < playerCorpses.Count; i++)
			{
                MyPlayerCorpse playerCorpse = (MyPlayerCorpse)playerCorpses[i];
                if (playerCorpse.Revived || (gottaBustGhosts && playerCorpse.spawningGhost))
				{
					array[(int)playerCorpse.Allegiance] = true;
                }
            }

            if (gottaBustGhosts && players.Count >= 1)
			{
                List<Entity> playerGhosts = this.Session.CurrentLevel[GameTags.PlayerGhost];
                for (int i = 0; i < playerGhosts.Count; i++)
				{
                    PlayerGhost playerGhost = (PlayerGhost)playerGhosts[i];
                    if (playerGhost.State != 3)
					{ // Ghost not dead
						array[(int)playerGhost.Allegiance] = true;
                    }
                }
            }

            if (array[0] == array[1])
			{
                surviving = Allegiance.Neutral;
            }
			else if (array[0])
			{
                surviving = Allegiance.Blue;
            }
			else
			{
                surviving = Allegiance.Red;
            }
            return !array[0] || !array[1];
        }
        
		public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
		{
			if (base.Session.MatchSettings.Mode == Modes.TeamDeathmatch)
			{
				Allegiance allegiance;
				if (this.TeamCheckForRoundOver(out allegiance))
				{
					base.Session.CurrentLevel.Ending = true;
				}
			}
			else if (
			  base.Session.MatchSettings.Mode == Modes.LastManStanding ||
			  base.Session.MatchSettings.Mode == Modes.HeadHunters
		  )
			{
				if (this.FFACheckForAllButOneDead())
				{
					base.Session.CurrentLevel.Ending = true;
				}
			}
		}
        
        public new void FinalKillTeams (PlayerCorpse corpse, Allegiance otherSpotlightTeam)
        {
            List<LevelEntity> list = new List<LevelEntity> ();
            for (int i = 0; i < 8; i++) {
                if (TFGame.Players [i] && this.Session.MatchSettings.Teams [i] == otherSpotlightTeam) {
                    this.Session.MatchStats [i].GotWin = true;
                    LevelEntity playerOrCorpse = this.Session.CurrentLevel.GetPlayerOrCorpse (i);
                    if (playerOrCorpse != null && playerOrCorpse != corpse) {
                        list.Add (playerOrCorpse);
                    }
                }
            }
            this.Session.CurrentLevel.LightingLayer.SetSpotlight (list.ToArray ());
            this.FinalKillNoSpotlight ();
        }

        public override void FinalKillNoSpotlight ()
        {
            this.Session.CurrentLevel.OrbLogic.DoSlowMoKill ();
            this.Session.MatchSettings.LevelSystem.StopVersusMusic ();
            Sounds.sfx_finalKill.Play (160f, 1f);
        }
	}
}
