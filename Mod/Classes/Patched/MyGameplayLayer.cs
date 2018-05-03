using Patcher;
using TowerFall;
using Monocle;
using System;
using System.Collections.Generic;

namespace Mod
{
  [Patch]
  public class MyGameplayLayer : GameplayLayer
  {
    public override void BatchedRender ()
    {
      base.BatchedRender();

      List<Entity> teamRevivers = base.Scene[GameTags.Dummy]; // Using Dummy tag for MyTeamReviver
      for (int i = 0; i < teamRevivers.Count; i++) {
        MyTeamReviver teamReviver = (MyTeamReviver)teamRevivers[i];
        teamReviver.HUDRender ();
      }
    }
  }
}
