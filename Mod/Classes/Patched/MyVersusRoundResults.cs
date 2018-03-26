using Patcher;
using TowerFall;
using System.Collections.Generic;

namespace Mod
{
  [Patch]
  public class MyVersusRoundResults : VersusRoundResults
  {
    private Modes _oldMode;

    public MyVersusRoundResults(Session session, List<EventLog> events)
      : base(session, events)
    {
      this._oldMode = session.MatchSettings.Mode;
      if (
        this._oldMode == RespawnRoundLogic.Mode ||
        this._oldMode == MobRoundLogic.Mode
      ) {
        session.MatchSettings.Mode = Modes.HeadHunters;
      }
    }

    public override void TweenOut()
    {
      this.session.MatchSettings.Mode = this._oldMode;
      base.TweenOut();
    }
  }
}
