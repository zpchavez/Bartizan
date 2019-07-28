using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Mod
{
  public class MyRosterPlayerButton : MyOptionsButton
  {
    public MyRosterPlayerButton (string title)
      : base (title)
    {
    }

    public void InitValue(string color)
    {
      this.SetCallbacks(
        delegate {
          this.State = color;
          this.CanLeft = true;
          this.CanRight = true;
        },
        delegate {
        // Left
        },
        delegate {
        // Right
        },
        null
      );
    }
  }
}
