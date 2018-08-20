using System;
using System.Collections.Generic;
using Monocle;
using Patcher;
using TowerFall;

namespace Mod
{
    [Patch]
    public class MyLastManStandingRoundLogic : LastManStandingRoundLogic
    {
        public MyLastManStandingRoundLogic(Session session) : base(session)
        {
        }
        
        public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
        {
            ((MyRoundLogic)base.Session.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
            if (this.wasFinalKill && base.Session.CurrentLevel.LivingPlayers == 0) {
                base.CancelFinalKill ();
            } else if (base.FFACheckForAllButOneDead ()) {
                int num = -1;
                
                List<Entity> players = this.Session.CurrentLevel[GameTags.Player];
                for (int i = 0; i < players.Count; i++)
                {
                    Player item = (Player)players[i];
                    if (!item.Dead) {
                        num = item.PlayerIndex;
                        break;
                    }
                }
                
                base.Session.CurrentLevel.Ending = true;
                if (num != -1 && base.Session.Scores [num] >= base.Session.MatchSettings.GoalScore - 1) {
                    this.wasFinalKill = true;
                    base.FinalKill (corpse, num);
                }
            }
        }
    }
}
