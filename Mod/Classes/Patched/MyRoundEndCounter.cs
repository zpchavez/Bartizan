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
