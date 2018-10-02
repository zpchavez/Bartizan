using System;
using Microsoft.Xna.Framework;
using Monocle;
using Patcher;
using TowerFall;
using System.Collections.Generic;

namespace Mod
{
    [Patch]
    public class MyTeamDeathmatchRoundLogic : TeamDeathmatchRoundLogic
    {
        public MyTeamDeathmatchRoundLogic(Session session)
            : base(session)
        {
        }

        public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
        {
            ((MyRoundLogic)base.Session.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
            Allegiance allegiance = default(Allegiance);
            if (this.wasFinalKill && base.Session.CurrentLevel.LivingPlayers == 0)
            {
                this.wasFinalKill = false;
                base.CancelFinalKill();
            }
            else if (((MyRoundLogic)base.Session.RoundLogic).TeamCheckForRoundOver(out allegiance))
            {
                base.Session.CurrentLevel.Ending = true;
                if (allegiance != Allegiance.Neutral && base.Session.Scores[(int)allegiance] >= base.Session.MatchSettings.GoalScore - 1)
                {
                    this.wasFinalKill = true;
                    base.FinalKillTeams(corpse, allegiance);
                }
            }
        }

        public void OnTeamRevive(Player player)
        {
            Allegiance allegiance = default(Allegiance);
            if (!((MyRoundLogic)base.Session.RoundLogic).TeamCheckForRoundOver(out allegiance))
            {
                base.Session.CurrentLevel.Ending = false;
            }
        }
    }
}
