using Patcher;
using TowerFall;
using Monocle;

namespace Mod
{
  [Patch]
  public class MyPlayerGhost : PlayerGhost
  {
    PlayerCorpse corpse;

    public MyPlayerGhost(PlayerCorpse corpse)
      : base(corpse)
    {
      this.corpse = corpse;
    }

    public override void Die(int killerIndex, Arrow arrow, Explosion explosion, ShockCircle circle)
    {
      base.Die(killerIndex, arrow, explosion, circle);
      var mobLogic = this.Level.Session.RoundLogic as MobRoundLogic;
      if (mobLogic != null) {
        // Ghosts treated as players in crawl mode
        mobLogic.OnPlayerDeath(
          null, this.corpse, this.PlayerIndex, DeathCause.Arrow, // FIXME
          this.Position, killerIndex
        );
      }
    }

    public override void Update()
    {
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).FastGhosts) {
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult * 1.5f, null);
        base.Update();
        typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult / 1.5f, null);
      } else {
        base.Update();
      }
    }
  }
}
