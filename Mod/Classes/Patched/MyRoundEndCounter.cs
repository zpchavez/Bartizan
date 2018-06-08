using TowerFall;
using Patcher;
using System.Collections.Generic;
using Monocle;

namespace Mod
{
  [Patch]
  public class MyRoundEndCounter : RoundEndCounter
  {
    public MyRoundEndCounter (Session session)
      : base(session)
    {
    }

    public override void Update() {
      base.Update();

      if (((MyMatchVariants)this.session.MatchSettings.Variants).GottaBustGhosts) {
        if (this.session.MatchSettings.TeamMode) {
          bool[] allegiances = new bool[2];
          // Check if there are players from one team and ghosts from another
          List<Entity> players = this.session.CurrentLevel[GameTags.Player];
          for (int i = 0; i < players.Count; i++) {
            MyPlayer player = (MyPlayer) players[i];
            if (!player.Dead) {
              allegiances [(int)player.Allegiance] = true;
            }
          }
          List<Entity> playerCorpses = this.session.CurrentLevel[GameTags.Corpse];
          for (int i = 0; i < playerCorpses.Count; i++) {
            MyPlayerCorpse playerCorpse = (MyPlayerCorpse) playerCorpses[i];
            if (playerCorpse.Revived) {
              allegiances [(int)playerCorpse.Allegiance] = true;
            }
          }
          if (allegiances[0] && allegiances[1] || (!allegiances[0] && !allegiances[1])) {
            // Either both teams still have players, or both teams have no players
            return;
          } else {
            List<Entity> playerGhosts = this.session.CurrentLevel[GameTags.PlayerGhost];
            for (int i = 0; i < playerGhosts.Count; i++) {
              PlayerGhost playerGhost = (PlayerGhost) playerGhosts[i];
              if (playerGhost.State != 3) { // Ghost not dead
                allegiances [(int)playerGhost.Allegiance] = true;
              }
            }
            if (allegiances[0] && allegiances[1]) {
              // There are ghosts from the opposing team
              this.ghostWaitCounter = 1;
            }
          }
        } else {
          this.ghostWaitCounter = 1;
        }
      }
    }
  }
}
