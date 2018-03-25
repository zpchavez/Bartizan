using Patcher;
using TowerFall;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

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

      // Check for tf-tracker-api.json
      TrackerMatchStats stats = new TrackerMatchStats();

      TFGame.Log(new Exception("Results"), false);
      TFGame.Log(new Exception("ROUND PLAYED " + ((MySession)this.session).RoundsPlayedThisMatch.ToString()), false);

      stats.rounds = ((MySession)this.session).RoundsPlayedThisMatch;
      for (int index = 0; index < this.session.MatchStats.Length; index++) {
        if (TFGame.Players[index]) {
          stats.kills[index] = (int)this.session.MatchStats[index].Kills.Kills;
          stats.deaths[index] = (int)this.session.MatchStats[index].Deaths.Kills;
          stats.wins[index] = this.session.MatchStats[index].Won ? 1 : 0;
        }
      }
      TFGame.Log(new Exception(stats.ToString()), false);
    }
  }
}
