using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;
using Patcher;
using System.Collections.Generic;

namespace Mod
{
  [Patch]
  public class MyRollcallElement : RollcallElement
  {
    public MyRollcallElement(int playerIndex) : base(playerIndex) { }

    public override void ForceStart()
    {
      MyVersusPlayerMatchResults.PlayerWins = new int[8];
      base.ForceStart();
    }

    public override void StartVersus()
    {
      MyVersusPlayerMatchResults.PlayerWins = new int[8];
      base.StartVersus();
    }
  }
}
