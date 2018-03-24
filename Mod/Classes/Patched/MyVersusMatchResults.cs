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
    public int PrevRoundsPlayed = 0;
    public int[] PrevArcherPlays;
    public int[] PrevArcherKills;
    public int[] PrevArcherDeaths;
    public int[] PrevArcherWins;

    public MyVersusMatchResults (Session session, VersusRoundResults roundResults) : base(session, roundResults)
    {
    }

    public override void Added ()
    {
      // Check for tf-tracker-api.json

      base.Added();
      TrackerMatchStats stats = new TrackerMatchStats();

      TFGame.Log(new Exception("Results"), false);

      if (PrevRoundsPlayed == 0) {
        stats.rounds = SessionStats.RoundsPlayed;
        for (int index = 0; index < SessionStats.ArcherPlays.Length; index++) {
          if (SessionStats.ArcherPlays[index] > 0) {
            ArcherColor color = (ArcherColor)index;
            stats.kills[index] = SessionStats.ArcherKills[index];
            stats.deaths[index] = SessionStats.ArcherDeaths[index];
            stats.wins[index] = SessionStats.ArcherWins[index];
          }
        }
      } else {
        for (int index = 0; index < SessionStats.ArcherPlays.Length; index++) {
          if (SessionStats.ArcherPlays[index] > 0) {
            ArcherColor color = (ArcherColor)index;
            stats.kills[index] = SessionStats.ArcherKills[index] - PrevArcherKills[index];
            stats.deaths[index] = SessionStats.ArcherDeaths[index] - PrevArcherDeaths[index];
            stats.wins[index] = SessionStats.ArcherWins[index] - PrevArcherWins[index];
          }
        }
      }

      TFGame.Log(new Exception(stats.ToString()), false);

      PrevRoundsPlayed = SessionStats.RoundsPlayed;
      PrevArcherPlays = SessionStats.ArcherPlays;
      PrevArcherKills = SessionStats.ArcherKills;
      PrevArcherDeaths = SessionStats.ArcherDeaths;
      PrevArcherWins = SessionStats.ArcherWins;
    }
  }
}
