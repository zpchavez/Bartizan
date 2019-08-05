using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using TowerFall;

namespace Mod
{
  public class RosterPage : ArchivesPage
  {
    public new RosterController Controller;

    private int pageIndex;

    public RosterPage (int pageIndex)
      : base (GetTitle(pageIndex))
    {
      this.CanLeave = true;
      this.pageIndex = pageIndex;
    }

    private static string GetTitle(int pageIndex)
    {
      return "PAGE " + (pageIndex + 1).ToString();
    }

    // public override void Added()
    // {
    //   base.Added();

    //   this.InitRosterOptions();
    // }

    // public void InitRosterOptions ()
    // {
    //   List<RosterPlayerButton> buttons = RosterButtonCreator.Create(this.pageIndex, this.trackerClient);

    //   for (int i = 0; i < buttons.Count; i++) {
    //     RosterPlayerButton optionsButton = buttons [i];
    //     optionsButton.TweenTo = new Vector2 (250f, (float)(65 + i * 15));
    //     optionsButton.Position = (optionsButton.TweenFrom = new Vector2 ((float)((i % 2 == 0) ? (-160) : 580), (float)(45 + i * 12)));
    //     if (i > 0) {
    //       optionsButton.UpItem = buttons [i - 1];
    //     }
    //     if (i < buttons.Count - 1) {
    //       optionsButton.DownItem = buttons [i + 1];
    //     }

    //     foreach (Component component in optionsButton.Components) {
    //       base.Add(component);
    //     }

    //     // this.Layers [optionsButton.LayerIndex].Add(optionsButton, false);
    //   }
    // }

    // public override void Update ()
    // {
    //   base.Update ();
    //   if (base.IsOnscreen) {
    //     // if (MenuInput.DownCheck) {
    //     //   this.scrollY = Math.Min (this.maxScrollY, this.scrollY + 4f * Engine.TimeMult);
    //     // } else if (MenuInput.UpCheck) {
    //     //   this.scrollY = Math.Max (0f, this.scrollY - 4f * Engine.TimeMult);
    //     // }
    //   }
    // }
  }
}
