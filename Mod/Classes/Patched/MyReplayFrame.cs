using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TowerFall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Patcher;
using Monocle;

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
