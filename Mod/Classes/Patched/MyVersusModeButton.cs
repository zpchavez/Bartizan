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
  public class MyVersusModeButton : VersusModeButton
  {
    static List<Modes> VersusModes = new List<Modes> {
      Modes.LastManStanding,
      Modes.HeadHunters,
      Modes.TeamDeathmatch,
      RespawnRoundLogic.Mode,
      MobRoundLogic.Mode,
    };

    public MyVersusModeButton(Vector2 position, Vector2 tweenFrom)
      : base(position, tweenFrom)
    {
    }

    public new static string GetModeName(Modes mode)
    {
      switch (mode) {
        case RespawnRoundLogic.Mode:
          return "RESPAWN";
        case MobRoundLogic.Mode:
          return "CRAWL";
        default:
          return VersusModeButton.GetModeName(mode);
      }
    }

    public new static Subtexture GetModeIcon(Modes mode)
    {
      switch (mode) {
        case RespawnRoundLogic.Mode:
          return TFGame.MenuAtlas["gameModes/respawn"];
        case MobRoundLogic.Mode:
          return TFGame.MenuAtlas["gameModes/crawl"];
        default:
          return VersusModeButton.GetModeIcon(mode);
      }
    }

    // completely re-write to make it enum-independent
    public override void Update()
    {
      // skip original implementation
      Patcher.Patcher.CallRealBase();

      Modes mode = MainMenu.VersusMatchSettings.Mode;
      if (this.Selected) {
        int idx = VersusModes.IndexOf(mode);
        if (idx < VersusModes.Count - 1 && MenuInput.Right) {
          MainMenu.VersusMatchSettings.Mode = VersusModes[idx + 1];
          Sounds.ui_move2.Play(160f, 1f);
          this.iconWiggler.Start();
          base.OnConfirm();
          this.UpdateSides();
        } else if (idx > 0 && MenuInput.Left) {
          MainMenu.VersusMatchSettings.Mode = VersusModes[idx - 1];
          Sounds.ui_move2.Play(160f, 1f);
          this.iconWiggler.Start();
          base.OnConfirm();
          this.UpdateSides();
        }
      }
    }

    public override void UpdateSides()
    {
      base.UpdateSides();
      this.DrawRight = (MainMenu.VersusMatchSettings.Mode < VersusModes[VersusModes.Count-1]);
    }
  }
}
