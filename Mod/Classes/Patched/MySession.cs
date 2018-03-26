using TowerFall;
using Patcher;

namespace Mod
{
  [Patch]
  public class MySession : Session
  {
    public int RoundsPlayedThisMatch;

    public MySession (MatchSettings settings) : base(settings)
    {
      RoundsPlayedThisMatch = 0;
    }
  }
}
