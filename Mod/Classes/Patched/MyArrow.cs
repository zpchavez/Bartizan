using TowerFall;
using Patcher;
using Monocle;

namespace Mod
{
  [Patch]
  public abstract class MyArrow : Arrow
  {
    const float AwfullySlowArrowMult = 0.2f;
    const float AwfullyFastArrowMult = 3.0f;

    public override void Added()
    {
      base.Added();

      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).AwfullyFastArrows) {
        this.NormalHitbox = new WrapHitbox(6f, 3f, -1f, -1f);
        this.otherArrowHitbox = new WrapHitbox(12f, 4f, -2f, -2f);
      }
    }

    public override void ArrowUpdate()
    {
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).AwfullySlowArrows) {
        // Engine.TimeMult *= AwfullySlowArrowMult;
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult * AwfullySlowArrowMult, null);
        base.ArrowUpdate();
        // Engine.TimeMult /= AwfullySlowArrowMult;
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult / AwfullySlowArrowMult, null);
      } else if (((MyMatchVariants)Level.Session.MatchSettings.Variants).AwfullyFastArrows) {
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult * AwfullyFastArrowMult, null);
        base.ArrowUpdate();
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult / AwfullyFastArrowMult, null);
      } else
        base.ArrowUpdate();
    }
  }
}
