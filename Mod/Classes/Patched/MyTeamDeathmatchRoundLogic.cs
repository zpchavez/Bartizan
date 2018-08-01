using System;
using Microsoft.Xna.Framework;
using Monocle;
using Patcher;
using TowerFall;
using System.Collections.Generic;

namespace Mod
{
	public class MyTeamDeathmatchRoundLogic : RoundLogic, GhostDeathInterface
    {
		public RoundEndCounter roundEndCounter;

        public bool done;

        public bool wasFinalKill;

		Session session;

        public MyTeamDeathmatchRoundLogic(Session session)
            : base(session, true)
        {
            this.roundEndCounter = new RoundEndCounter(session);
			this.session = session;
        }

        public override void OnLevelLoadFinish()
        {
            base.OnLevelLoadFinish();
            base.Session.CurrentLevel.Add(new VersusStart(base.Session));
            base.SpawnPlayersTeams();
            base.Players = TFGame.PlayerAmount;
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();
            base.SpawnTreasureChestsVersus();
        }

        public override void OnUpdate()
        {
            SessionStats.TimePlayed += Engine.DeltaTicks;
            base.OnUpdate();
            if (base.RoundStarted && !this.done && base.Session.CurrentLevel.Ending && base.Session.CurrentLevel.CanEnd)
			{
                if (!this.roundEndCounter.Finished)
				{
                    this.roundEndCounter.Update();
                }
				else
				{
                    this.done = true;
                    if (base.Session.CurrentLevel.Players.Count > 0)
					{
                        base.AddScore((int)(base.Session.CurrentLevel.Players[0] as Player).Allegiance, 1);
                    }
                    base.InsertCrownEvent();
                    base.Session.EndRound();
                }
            }
        }

        public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause deathType, Vector2 position, int killerIndex)
        {
            base.OnPlayerDeath(player, corpse, playerIndex, deathType, position, killerIndex);
            Allegiance allegiance = default(Allegiance);
            if (this.wasFinalKill && base.Session.CurrentLevel.LivingPlayers == 0)
			{
                this.wasFinalKill = false;
                base.CancelFinalKill();
            }
			else if (base.TeamCheckForRoundOver(out allegiance))
			{
                base.Session.CurrentLevel.Ending = true;
                if (allegiance != Allegiance.Neutral && base.Session.Scores[(int)allegiance] >= base.Session.MatchSettings.GoalScore - 1)
				{
                    this.wasFinalKill = true;
                    base.FinalKillTeams(corpse, allegiance);
                }
            }
        }

        public void CheckForWin()
        {
            Allegiance allegiance = default(Allegiance);
            if (base.TeamCheckForRoundOver(out allegiance))
			{
                base.Session.CurrentLevel.Ending = true;
            }
        }

		public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
		{
            Allegiance allegiance = default(Allegiance);
            if (this.wasFinalKill && base.Session.CurrentLevel.LivingPlayers == 0)
            {
                this.wasFinalKill = false;
                base.CancelFinalKill();
            }
            else if (base.TeamCheckForRoundOver(out allegiance))
            {
                base.Session.CurrentLevel.Ending = true;
                if (allegiance != Allegiance.Neutral && base.Session.Scores[(int)allegiance] >= base.Session.MatchSettings.GoalScore - 1)
                {
                    this.wasFinalKill = true;
                    base.FinalKillTeams(corpse, allegiance);
                }
            }
		}
	}
}
