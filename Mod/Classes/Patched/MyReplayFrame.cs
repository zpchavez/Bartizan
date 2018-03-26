using TowerFall;
using Microsoft.Xna.Framework;
using Patcher;

namespace Mod
{
  [Patch]
  public class MyReplayFrame : ReplayFrame
  {
    public override void Record (float timeSinceLastFrame, long ticksSinceLastFrame)
    {
      base.Record(timeSinceLastFrame, ticksSinceLastFrame);
      // Undo screen offset to fix glitched out frames during screen shake
      this.ScreenOffsetAdd = new Vector2(0, 0);
    }
  }
}
