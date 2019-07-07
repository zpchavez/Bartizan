using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;
using Patcher;
using System.Collections.Generic;

namespace Mod
{
  [Patch]
  public class MyVersusPlayerMatchResults : VersusPlayerMatchResults
  {
    public static int[] PlayerWins;

    OutlineText winsText;

    // For 8-player
    public MyVersusPlayerMatchResults(Session session, VersusMatchResults matchResults, int playerIndex, bool small, Vector2 tweenFrom, Vector2 tweenTo, List<AwardInfo> awards)
      : base(session, matchResults, playerIndex, small, tweenFrom, tweenTo, awards)
    {
      this.showWinCount();
    }

    // For 4-player
    public MyVersusPlayerMatchResults(Session session, VersusMatchResults matchResults, int playerIndex, Vector2 tweenFrom, Vector2 tweenTo, List<AwardInfo> awards)
      : base(session, matchResults, playerIndex, small, tweenFrom, tweenTo, awards)
    {
      this.showWinCount();
    }

    public void showWinCount()
    {
      if (session.MatchStats[playerIndex].Won) {
        PlayerWins[playerIndex]++;
      }

      if (PlayerWins[playerIndex] > 0) {
        winsText = new OutlineText(TFGame.Font, PlayerWins[playerIndex].ToString(), this.gem.Position);
        winsText.Color = Color.White;
        winsText.OutlineColor = Color.Black;
        this.Add(winsText);
      }
    }

    public override void Render()
    {
      base.Render();
      if (winsText != null)
        winsText.Render();
    }
  }
}
