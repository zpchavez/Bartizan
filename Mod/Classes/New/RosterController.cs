using Microsoft.Xna.Framework;
using Monocle;
using System;
using TowerFall;

namespace Mod
{
  public class RosterController : MenuItem
  {
    public const float atY = 45f;

    public const float arrowXOffset = 80f;

    public const int PAGE_TIME = 20;

    public OutlineText title;

    public Image leftArrow;

    public Image rightArrow;

    public RosterPage[] pages;

    public int previousPageIndex;

    public int pageIndex;

    public Tween moveTween;

    public Wiggler arrowWiggle;

    public SineWave arrowSine;

    private Action onPageChange;

    public bool leftWiggle;

    public RosterController ()
      : base (Vector2.Zero)
    {
      this.title = new OutlineText (TFGame.Font, "", Vector2.Zero, Text.HorizontalAlign.Center, Text.VerticalAlign.Center);
      this.title.Scale = Vector2.One * 2f;
      base.Add (this.title);
      this.leftArrow = new Image (TFGame.MenuAtlas ["portraits/arrow"], null);
      this.leftArrow.CenterOrigin ();
      this.leftArrow.FlipX = true;
      this.leftArrow.Scale = Vector2.One * 2f;
      base.Add (this.leftArrow);
      this.rightArrow = new Image (TFGame.MenuAtlas ["portraits/arrow"], null);
      this.rightArrow.CenterOrigin ();
      this.rightArrow.Scale = Vector2.One * 2f;
      base.Add (this.rightArrow);
      this.arrowWiggle = Wiggler.Create (PAGE_TIME, 6f, Ease.CubeIn, null, false, false);
      base.Add (this.arrowWiggle);
      base.Add (this.arrowSine = new SineWave (120));
    }

    public void SetPages (params RosterPage[] pages)
    {
      this.pages = pages;
      this.title.DrawText = pages [0].Title;
      for (int i = 0; i < pages.Length; i++) {
        pages [i].Controller = this;
        pages [i].X = this.GetPageX (i);
      }
    }

    public void SetOnPageChange(Action callback)
    {
      this.onPageChange = callback;
    }

    public override void Update ()
    {
      base.Update ();
      if (MenuInput.Right && this.pageIndex < this.pages.Length - 1 && this.pages [this.pageIndex].CanLeave) {
        this.leftWiggle = false;
        this.previousPageIndex = this.pageIndex;
        this.pageIndex++;
        this.TweenPages ();
        // if (this.onPageChange != null) {
        //   this.onPageChange();
        // }
      } else if (MenuInput.Left && this.pageIndex > 0 && this.pages [this.pageIndex].CanLeave) {
        this.leftWiggle = true;
        this.previousPageIndex = this.pageIndex;
        this.pageIndex--;
        this.TweenPages ();
        // if (this.onPageChange != null) {
        //   this.onPageChange();
        // }
      }
    }

    public override void Render ()
    {
      if (this.leftWiggle) {
        this.title.X = this.arrowWiggle.Value * -6f;
      } else {
        this.title.X = this.arrowWiggle.Value * 6f;
      }
      if (this.pages [this.pageIndex].CanLeave) {
        this.leftArrow.X = -arrowXOffset + this.arrowSine.Value * 5f + (this.leftWiggle ? (this.arrowWiggle.Value * 10f) : 0f);
        this.rightArrow.X = arrowXOffset - this.arrowSine.Value * 5f - ((!this.leftWiggle) ? (this.arrowWiggle.Value * 10f) : 0f);
        this.leftArrow.Color = ((this.pageIndex == 0) ? Color.Transparent : Color.White);
        this.rightArrow.Color = ((this.pageIndex == this.pages.Length - 1) ? Color.Transparent : Color.White);
      } else {
        this.leftArrow.Color = (this.rightArrow.Color = Color.Transparent);
      }
      base.Render ();
    }

    public void TweenPages ()
    {
      if ((bool)this.moveTween && base.Contains (this.moveTween)) {
        base.Remove (this.moveTween);
      }
      Sounds.ui_move1.Play (210f, 1f);
      this.arrowWiggle.Start ();
      this.title.DrawText = this.pages [this.pageIndex].Title;
      this.moveTween = Tween.Create (Tween.TweenMode.Oneshot, Ease.CubeInOut, PAGE_TIME, true);
      this.moveTween.OnUpdate = delegate (Tween t) {
        for (int i = 0; i < this.pages.Length; i++) {
          this.pages [i].X = MathHelper.Lerp (this.GetPageOldX (i), this.GetPageX (i), t.Eased);
        }
      };
      this.moveTween.OnComplete = delegate {
        this.moveTween = null;
        this.pages [this.pageIndex].OnTweenIn ();
      };
      base.Add (this.moveTween);
    }

    public float GetPageX (int index)
    {
      // return (float)(210 + 420 * (index - this.pageIndex));
      return (float)(420 * (index - this.pageIndex));
    }

    public float GetPageOldX (int index)
    {
      return (float)(210 + 420 * (index - this.previousPageIndex));
    }

    public override void TweenIn ()
    {
      base.Position = new Vector2 (580f, atY);
      Tween tween = Tween.Create (Tween.TweenMode.Oneshot, Ease.CubeOut, PAGE_TIME, true);
      tween.OnUpdate = delegate (Tween t) {
        base.Position = Vector2.Lerp (new Vector2 (580f, atY), new Vector2 (210f, atY), t.Eased);
      };
      base.Add (tween);
    }

    public override void TweenOut ()
    {
      Tween tween = Tween.Create (Tween.TweenMode.Oneshot, Ease.CubeIn, 12, true);
      tween.OnUpdate = delegate (Tween t) {
        base.Position = Vector2.Lerp (new Vector2 (210f, atY), new Vector2 (-210f, atY), t.Eased);
      };
      base.Add (tween);
    }

    public override void OnSelect ()
    {
      throw new NotImplementedException ();
    }

    public override void OnDeselect ()
    {
      throw new NotImplementedException ();
    }

    public override void OnConfirm ()
    {
      throw new NotImplementedException ();
    }
  }
}
