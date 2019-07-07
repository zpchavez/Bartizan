using Microsoft.Xna.Framework;
using Monocle;
using System;
using TowerFall;
using Patcher;

namespace Mod
{
  [Patch]
  public class MyPlayerIndicator : PlayerIndicator
  {
    public MyPlayerIndicator (Vector2 offset, int playerIndex, bool crown)
      : base (offset, playerIndex, crown)
    {
      if (MyGlobals.playerNames[playerIndex] is string) {
        this.text = MyGlobals.playerNames[playerIndex].ToUpper();
      } else {
        this.text = "P" + (playerIndex + 1).ToString();
      }
    }

    public override void Render ()
    {
      Color color = this.colorSwitch ? ArcherData.Archers [this.characterIndex].ColorB : ArcherData.Archers [this.characterIndex].ColorA;
      Vector2 value = base.Entity.Position + this.offset + new Vector2 (0f, -32f);
      value.Y = Math.Max (10f, value.Y);
      value.Y += this.sine.Value * 3f;
      Vector2 vector = TFGame.Font.MeasureString (this.text) * 2f;
      if (this.crown) {
        Draw.OutlineTextureCentered (TFGame.Atlas ["versus/crown"], value + new Vector2 (0f, -12f), Color.White);
      }
      Draw.OutlineTextCentered (TFGame.Font, this.text, value + new Vector2 (1f, 0f), color, 1.0f);
      Draw.OutlineTextureCentered (TFGame.Atlas ["versus/playerIndicator"], value + new Vector2 (0f, 8f), color);
    }
  }
}
