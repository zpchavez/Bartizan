using TowerFall;
using Patcher;

namespace Mod
{
  [Patch]
  public class MyRollcallElement : RollcallElement
  {
    public MyRollcallElement(int playerIndex) : base(playerIndex) { }

    public override void ForceStart()
    {
      MyVersusPlayerMatchResults.PlayerWins = new int[MyGlobals.MAX_PLAYERS];
      base.ForceStart();
    }

    public override void StartVersus()
    {
      MyVersusPlayerMatchResults.PlayerWins = new int[MyGlobals.MAX_PLAYERS];
      base.StartVersus();
    }
  }
}
