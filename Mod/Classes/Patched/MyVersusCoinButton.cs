using Patcher;
using TowerFall;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Mod
{
  [Patch]
  public class MyVersusCoinButton : VersusCoinButton
  {
    public MyVersusCoinButton(Vector2 position, Vector2 tweenFrom)
      : base(position, tweenFrom)
    {
    }

    public override void Render()
    {
      var mode = MainMenu.VersusMatchSettings.Mode;
      if (mode == RespawnRoundLogic.Mode
        || mode == MobRoundLogic.Mode
      ) {
        MainMenu.VersusMatchSettings.Mode = Modes.HeadHunters;
        base.Render();
        MainMenu.VersusMatchSettings.Mode = mode;
      } else {
        base.Render();
      }
    }
  }
}
