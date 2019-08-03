using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Mod
{
  public class MyOptionsButton : MenuItem
  {
    public static readonly Color SelectedColor = Calc.HexToColor ("D8F878");

    public static readonly Color NotSelectedColor = Calc.HexToColor ("3CBCFC");

    public Vector2 TweenFrom;

    public Vector2 TweenTo;

    public string State;

    public bool CanLeft;

    public bool CanRight;

    public string title;

    public Image rightArrow;

    public Image leftArrow;

    public Action setProperties;

    public Action onRight;

    public Action onLeft;

    public Func<bool> onConfirm;

    public Wiggler changedWiggler;

    public Wiggler selectedWiggler;

    public SineWave sine;

    public int wiggleDir = 1;

    public float canPress;

    public MyOptionsButton (string title)
      : base (Vector2.Zero)
    {
      this.title = title;
      this.changedWiggler = Wiggler.Create (30, 5f, null, null, false, false);
      base.Add (this.changedWiggler);
      this.selectedWiggler = Wiggler.Create (20, 4f, null, null, false, false);
      base.Add (this.selectedWiggler);
      this.sine = new SineWave (120);
      base.Add (this.sine);
      this.rightArrow = new Image (TFGame.MenuAtlas ["portraits/arrow"], null);
      this.rightArrow.CenterOrigin ();
      this.rightArrow.Visible = false;
      base.Add (this.rightArrow);
      this.leftArrow = new Image (TFGame.MenuAtlas ["portraits/arrow"], null);
      this.leftArrow.CenterOrigin ();
      this.leftArrow.FlipX = true;
      this.leftArrow.Visible = false;
      base.Add (this.leftArrow);
    }

    public void SetCallbacks (Action setProperties, Action onLeft, Action onRight, Func<bool> onConfirm)
    {
      this.setProperties = setProperties;
      this.onRight = onRight;
      this.onLeft = onLeft;
      this.onConfirm = onConfirm;
      setProperties ();
      if (onLeft == null && onRight == null) {
        this.CanLeft = (this.CanRight = false);
      }
    }

    public void SetCallbacks (Action onConfirm)
    {
      this.setProperties = delegate {
        this.State = "";
      };
      this.onLeft = null;
      this.onRight = null;
      this.onConfirm = delegate {
        onConfirm ();
        return true;
      };
      this.setProperties ();
      this.CanLeft = (this.CanRight = false);
    }

    public override void Update ()
    {
      if (this.canPress > 0f) {
        this.canPress -= Engine.TimeMult;
      }
      base.Update ();
      if (base.Selected) {
        if (MenuInput.Right && this.CanRight) {
          this.onRight ();
          this.setProperties ();
          this.changedWiggler.Start ();
          this.wiggleDir = 1;
          Sounds.ui_move1.Play (210f, 1f);
        } else if (MenuInput.Left && this.CanLeft) {
          this.onLeft ();
          this.setProperties ();
          this.changedWiggler.Start ();
          this.wiggleDir = -1;
          Sounds.ui_move1.Play (210f, 1f);
        }
      }
    }

    public override void Render ()
    {
      Vector2 vector = new Vector2 (30f + 2f * this.changedWiggler.Value * (float)this.wiggleDir, 0f);
      Color color = base.Selected ? MyOptionsButton.SelectedColor : MyOptionsButton.NotSelectedColor;
      Draw.OutlineTextJustify (TFGame.Font, this.title, base.Position + new Vector2 (-5f, 0f) + new Vector2 (5f * this.selectedWiggler.Value, 0f), color, Color.Black, new Vector2 (1f, 0.5f), 1f);
      if (this.State == "ON") {
        Draw.OutlineTextureCentered (TFGame.MenuAtlas ["optionOn"], base.Position + vector, color);
      } else if (this.State == "OFF") {
        Draw.OutlineTextureCentered (TFGame.MenuAtlas ["optionOff"], base.Position + vector, color);
      } else {
        Draw.OutlineTextJustify (TFGame.Font, this.State, base.Position + vector, color, Color.Black, Vector2.One * 0.5f, 1f);
      }
      if (this.onLeft != null) {
        this.leftArrow.Visible = (this.rightArrow.Visible = base.Selected);
        this.leftArrow.Color = Color.White * (this.CanLeft ? 1f : 0.3f);
        this.rightArrow.Color = Color.White * (this.CanRight ? 1f : 0.3f);
        this.leftArrow.Position = vector + Vector2.UnitX * (-20f + -3f * this.sine.Value + ((this.wiggleDir == -1) ? (this.changedWiggler.Value * -2f) : 0f));
        this.rightArrow.Position = vector + Vector2.UnitX * (20f + 3f * this.sine.Value + ((this.wiggleDir == 1) ? (this.changedWiggler.Value * 2f) : 0f));
      } else {
        this.leftArrow.Visible = (this.rightArrow.Visible = false);
      }
      base.Render ();
    }

    public override void TweenIn ()
    {
      Tween tween = Tween.Create (Tween.TweenMode.Oneshot, Ease.CubeOut, 20, true);
      tween.OnUpdate = delegate (Tween t) {
        base.Position = Vector2.Lerp (this.TweenFrom, this.TweenTo, t.Eased);
      };
      base.Add (tween);
    }

    public override void TweenOut ()
    {
      Tween tween = Tween.Create (Tween.TweenMode.Oneshot, Ease.CubeIn, 12, true);
      tween.OnUpdate = delegate (Tween t) {
        base.Position = Vector2.Lerp (this.TweenTo, this.TweenFrom, t.Eased);
      };
      base.Add (tween);
    }

    public override void OnSelect ()
    {
      this.selectedWiggler.Start ();
    }

    public override void OnDeselect ()
    {
    }

    public override void OnConfirm ()
    {
      if (this.canPress <= 0f && this.onConfirm != null) {
        this.changedWiggler.Start ();
        if (this.onConfirm ()) {
          Sounds.ui_subclickOn.Play (210f, 1f);
        } else {
          Sounds.ui_subclickOff.Play (210f, 1f);
        }
        this.setProperties ();
        this.canPress = 10f;
      }
    }
  }
}
