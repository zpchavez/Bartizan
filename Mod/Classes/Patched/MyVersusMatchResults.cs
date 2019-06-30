using Patcher;
using TowerFall;
using SDL2;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Mod
{
  [Patch]
  public class MyVersusMatchResults : VersusMatchResults
  {

    public MyVersusMatchResults (Session session, VersusRoundResults roundResults) : base(session, roundResults)
    {
    }

    public override void Added ()
    {
      base.Added();

      TrackerApiClient client = new TrackerApiClient();

      if (client.IsSetup()) {
        TrackerMatchStats stats = new TrackerMatchStats();
        stats.rounds = ((MySession)this.session).RoundsPlayedThisMatch;
        for (int index = 0; index < this.session.MatchStats.Length; index++) {
          if (TFGame.Players[index]) {
            stats.kills[index] = (int)this.session.MatchStats[index].Kills.Kills;
            stats.deaths[index] =
              (int)this.session.MatchStats[index].Deaths.Kills +
              (int)this.session.MatchStats[index].Deaths.SelfKills +
              (int)this.session.MatchStats[index].Deaths.TeamKills;
            stats.wins[index] = this.session.MatchStats[index].Won ? 1 : 0;
            stats.selfs[index] = (int)this.session.MatchStats[index].Kills.SelfKills;
            stats.teamKills[index] = (int)this.session.MatchStats[index].Kills.TeamKills;
            stats.revives[index] = (int)this.session.MatchStats[index].Revives;
            stats.killsAsGhost[index] = (int)this.session.MatchStats[index].KillsAsGhost;
            stats.ghostKills[index] = (int)this.session.MatchStats[index].GhostKills;
            stats.miracles[index] = (int)((MySession)(this.session)).MyMatchStats[index].MiracleCatches;
          }
        }

        client.SaveStats(stats);
      }
    }
  }
}
