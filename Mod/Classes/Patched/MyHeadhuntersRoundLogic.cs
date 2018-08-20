using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using Patcher;
using TowerFall;

namespace Mod
{
    [Patch]
    public class MyHeadhuntersRoundLogic : HeadhuntersRoundLogic
    {
		public MyHeadhuntersRoundLogic(Session session) : base(session)
		{
		}
        
        public override bool OtherPlayerCouldWin (int playerIndex)
        {
            if (base.Session.Scores [playerIndex] < base.Session.MatchSettings.GoalScore || base.Session.GetScoreLead (playerIndex) <= 0) {
                return true;
            }
            int num = base.Session.GetHighestScore () - (base.Session.CurrentLevel.LivingPlayers);
            for (int i = 0; i < 8; i++) {
                if (TFGame.Players [i] && i != playerIndex) {
                    Player player = base.Session.CurrentLevel.GetPlayer (i);
					if (((MyMatchVariants)base.Session.MatchSettings.Variants).GottaBustGhosts)
					{
						if (player == null)
						{
							List<Entity> corpses = base.Session.CurrentLevel[GameTags.Corpse];
							for (int j = 0; j < corpses.Count; j++)
							{
								MyPlayerCorpse corpse = (MyPlayerCorpse)corpses[j];
                                if (corpse.PlayerIndex == i && (corpse.hasGhost || corpse.spawningGhost) && base.Session.Scores[i] >= num)
								{
									return true;
								}
							}
						}
						if (player != null && (!player.Dead || ((MyPlayer)player).spawningGhost) && base.Session.Scores[i] >= num)
						{
							return true;
						}
					}
					else
					{
						if (player != null && !player.Dead && base.Session.Scores[i] >= num)
						{
							return true;
						}
					}
                }
            }
            return false;
        }

		public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
		{
			((MyRoundLogic)base.Session.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
            int winner = base.Session.GetWinner ();
            if (((MyRoundLogic)base.Session.RoundLogic).FFACheckForAllButOneDead ()) {
                base.Session.CurrentLevel.Ending = true;
                if (winner != -1 && !this.wasFinalKill) {
                    this.wasFinalKill = true;
                    base.FinalKill (corpse, winner);
                }
            } else if (!this.wasFinalKill && winner != -1 && !this.OtherPlayerCouldWin (winner)) {
                base.Session.CurrentLevel.Ending = true;
                this.wasFinalKill = true;
                base.FinalKill (corpse, winner);
            }
		}
	}
}
