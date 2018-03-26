using TowerFall;
using Patcher;

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
        this.ghostWaitCounter = 1;
      }
    }
  }
}
