using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using Newtonsoft.Json.Linq;

namespace Mod
{
  public class RosterPlayerButton : MyOptionsButton
  {
    public Subtexture[] icons;

    public Image selectedIcon;

    public int value;

    public bool editing;

    public bool enableMenuBackOnNextTick;

    public MenuItem OriginalUpItem;

    public MenuItem OriginalDownItem;

    int playerId;

    public RosterPlayerButton (JObject player) : base(GetNameFromPlayer(player))
    {
      this.icons = new Subtexture[10];
      this.playerId = player["id"];
      for (int i = 0; i < 10; i++) {
        this.icons [i] = new Subtexture (TFGame.MenuAtlas ["seedDigits"], i * 12, 0, 12, 10);
      }
      this.selectedIcon = new Image (this.icons[0], null);
      this.selectedIcon.CenterOrigin();
      this.selectedIcon.Position = this.Position + new Vector2 (29f, 0f);
      this.selectedIcon.Visible = true;
      this.Add(this.selectedIcon);
      JToken colorToken = player["color"];
      if (colorToken.Type != JTokenType.Null) {
        string colorString = player.Value<string>("color");
        ArcherColor color = (ArcherColor)Enum.Parse(typeof(ArcherColor), colorString, false);
        this.value = (int)color + 1;
      } else {
        this.value = 0;
      }
      this.UpdateIcon();
      this.InitCallbacks();
    }

     private static string GetNameFromPlayer(JObject player)
     {
       return player.Value<string>("name").ToUpper();
     }

    public void InitCallbacks()
    {
      this.SetCallbacks(
        delegate {
          this.CanLeft = this.editing;
          this.CanRight = this.editing;
        },
        delegate {
          if (this.value == 0) {
            this.value = 9;
          } else {
            this.value -= 1;
          }
          this.UpdateIcon();
        },
        delegate {
          if (this.value == 9) {
            this.value = 0;
          } else {
            this.value += 1;
          }
          this.UpdateIcon();
        },
        delegate {
          this.ToggleEditMode();
          return true;
        }
      );
    }

    public void ToggleEditMode() {
      this.editing = !this.editing;
      if (this.editing) {
        this.OriginalUpItem = this.UpItem;
        this.OriginalDownItem = this.DownItem;
        // Prevent changing options or going back while editing
        this.UpItem = null;
        this.DownItem = null;
        this.MainMenu.CanAct = false;
      } else {
        this.UpItem = this.OriginalUpItem;
        this.DownItem = this.OriginalDownItem;
        this.enableMenuBackOnNextTick = true;
      }
    }

    public override void Update ()
    {
      base.Update ();

      if (this.enableMenuBackOnNextTick) {
        this.MainMenu.CanAct = true;
        this.enableMenuBackOnNextTick = false;
      }

      if (base.Selected) {
        if (MenuInput.Back && this.editing) {
          this.ToggleEditMode();
          this.value = 0;
          this.UpdateIcon();
        }
      }
    }

    public void UpdateIcon() {
      this.selectedIcon.SwapSubtexture(this.icons[this.value], null);
    }

    public override void Render ()
    {
      Color color = base.Selected ? MyOptionsButton.SelectedColor : MyOptionsButton.NotSelectedColor;
      Draw.OutlineTextJustify (TFGame.Font, this.title, base.Position + new Vector2 (-5f, 0f) + new Vector2 (5f * this.selectedWiggler.Value, 0f), color, Color.Black, new Vector2 (1f, 0.5f), 1f);

      Vector2 vector = new Vector2 (30f + 2f * this.changedWiggler.Value * (float)this.wiggleDir, 0f);
      if (this.onLeft != null) {
        this.leftArrow.Visible = (this.rightArrow.Visible = base.Selected && this.editing);
        this.leftArrow.Color = Color.White * (this.CanLeft ? 1f : 0.3f);
        this.rightArrow.Color = Color.White * (this.CanRight ? 1f : 0.3f);
        this.leftArrow.Position = vector + Vector2.UnitX * (-20f + -3f * this.sine.Value + ((this.wiggleDir == -1) ? (this.changedWiggler.Value * -2f) : 0f));
        this.rightArrow.Position = vector + Vector2.UnitX * (20f + 3f * this.sine.Value + ((this.wiggleDir == 1) ? (this.changedWiggler.Value * 2f) : 0f));
      } else {
        this.leftArrow.Visible = (this.rightArrow.Visible = false);
      }

      // Since we can't call base.Render without unwanted effects, run this code from the base Entity class
      if (this.Components != null) {
        foreach (Component component in this.Components) {
          if (component.Visible) {
            component.Render ();
          }
        }
      }
    }
  }
}
