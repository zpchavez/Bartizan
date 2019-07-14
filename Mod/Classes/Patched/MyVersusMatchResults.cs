using Patcher;
using TowerFall;
using SDL2;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

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
        JObject stats = new JObject();
        stats["rounds"] = ((MySession)this.session).RoundsPlayedThisMatch;
        stats["kills"] = new JObject();
        stats["deaths"] = new JObject();
        stats["wins"] = new JObject();
        stats["selfs"] = new JObject();
        stats["team_kills"] = new JObject();
        stats["revives"] = new JObject();
        stats["kills_as_ghost"] = new JObject();
        stats["ghost_kills"] = new JObject();
        stats["miracles"] = new JObject();
        stats["map"] = this.session.MatchSettings.LevelSystem.Theme.Name;
        for (int index = 0; index < this.session.MatchStats.Length; index++) {
          if (TFGame.Players[index]) {
            string color = ((ArcherColor)TFGame.Characters[index]).ToString();
            stats["kills"][color] = (int)this.session.MatchStats[index].Kills.Kills;
            stats["deaths"][color] =
              (int)this.session.MatchStats[index].Deaths.Kills +
              (int)this.session.MatchStats[index].Deaths.SelfKills +
              (int)this.session.MatchStats[index].Deaths.TeamKills;
            stats["wins"][color] = this.session.MatchStats[index].Won ? 1 : 0;
            stats["selfs"][color] = (int)this.session.MatchStats[index].Kills.SelfKills;
            stats["team_kills"][color] = (int)this.session.MatchStats[index].Kills.TeamKills;
            stats["revives"][color] = (int)this.session.MatchStats[index].Revives;
            stats["kills_as_ghost"][color] = (int)this.session.MatchStats[index].KillsAsGhost;
            stats["ghost_kills"][color] = (int)this.session.MatchStats[index].GhostKills;
            stats["miracles"][color] = (int)((MySession)(this.session)).MyMatchStats[index].MiracleCatches;
          }
        }

        client.SaveStats(stats);
      }
    }
  }
}
