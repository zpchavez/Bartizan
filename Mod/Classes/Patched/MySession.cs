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
  public class MySession : Session
  {
    public int RoundsPlayedThisMatch;

    public MySession (MatchSettings settings) : base(settings)
    {
      RoundsPlayedThisMatch = 0;
    }
  }
}
