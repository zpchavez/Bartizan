using TowerFall;
using Patcher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Mod
{
  [Patch]
  class MyMainMenu : MainMenu
  {
    public const int ROSTER = 16;

    private TrackerApiClient trackerClient;

    public MyMainMenu (MenuState state) : base(state)
    {
      trackerClient = new TrackerApiClient();
      if (trackerClient.IsSetup()) {
        trackerClient.GetRoster();
      }
    }

    public override void CreateMain ()
    {
      BladeButton rosterButton = null;
      BladeButton quitButton = null;
      List<MenuItem> list = new List<MenuItem> ();
      FightButton fightButton = new FightButton (new Vector2 (210f, 160f), new Vector2 (210f, 300f));
      list.Add (fightButton);
      ArchivesButton archivesButton = new ArchivesButton (new Vector2 (370f, 210f), new Vector2 (370f, 300f));
      list.Add (archivesButton);
      BladeButton optionsButton;
      BladeButton creditsButton;
      if (MainMenu.NoQuit) {
        if (trackerClient.IsSetup()) {
          rosterButton = new BladeButton (188f, "ROSTER", this.Roster);
          list.Add(rosterButton);
        }
        optionsButton = new BladeButton (206f, "OPTIONS", this.MainOptions);
        list.Add (optionsButton);
        creditsButton = new BladeButton (224f, "CREDITS", this.MainCredits);
        list.Add (creditsButton);
      } else {
        if (trackerClient.IsSetup()) {
          rosterButton = new BladeButton (174f, "ROSTER", this.Roster);
          list.Add(rosterButton);
        }
        optionsButton = new BladeButton (192f, "OPTIONS", this.MainOptions);
        list.Add (optionsButton);
        creditsButton = new BladeButton (210f, "CREDITS", this.MainCredits);
        list.Add (creditsButton);
        quitButton = new BladeButton (228f, "QUIT", this.MainQuit);
        list.Add (quitButton);
      }

      for (int i = 0; i < list.Count; i++) {
        MenuItem item = list[i];
        this.Layers [item.LayerIndex].Add(item, false);
      }

      fightButton.RightItem = archivesButton;
      if (trackerClient.IsSetup()) {
        optionsButton.UpItem = rosterButton;
        rosterButton.UpItem = fightButton;
        rosterButton.DownItem = optionsButton;
        fightButton.DownItem = rosterButton;
        fightButton.LeftItem = rosterButton;
      } else {
        optionsButton.UpItem = fightButton;
        fightButton.DownItem = optionsButton;
        fightButton.LeftItem = optionsButton;
      }
      optionsButton.DownItem = creditsButton;
      optionsButton.RightItem = fightButton;
      creditsButton.UpItem = optionsButton;
      creditsButton.RightItem = fightButton;
      if (!MainMenu.NoQuit) {
        creditsButton.DownItem = quitButton;
        quitButton.UpItem = creditsButton;
        quitButton.RightItem = fightButton;
      }
      archivesButton.LeftItem = fightButton;
      archivesButton.UpItem = fightButton;
      if (this.OldState == (MenuState)ROSTER) {
        this.ToStartSelected = rosterButton;
      } else if (this.OldState == MenuState.Options) {
        this.ToStartSelected = optionsButton;
      } else if (this.OldState == MenuState.Archives) {
        this.ToStartSelected = archivesButton;
      } else if (this.OldState == MenuState.Credits) {
        this.ToStartSelected = creditsButton;
      } else {
        this.ToStartSelected = fightButton;
      }
      this.BackState = MenuState.PressStart;
      this.TweenBGCameraToY (0);
      MainMenu.CurrentMatchSettings = null;
    }

    public override void CallStateFunc (string name, MenuState state)
    {
      if (state == (MenuState)ROSTER) {
        if (name == "Create") {
          this.CreateRoster();
        } else if (name == "Destroy") {
          this.DestroyRoster();
        }
      } else {
        MethodInfo method = typeof(MainMenu).GetMethod (name + state.ToString ());
        if (method != (MethodInfo)null) {
          method.Invoke (this, new object[0]);
        }
      }
    }

    public void CreateRoster()
    {
      if (MyGlobals.roster != null) {
        List<RosterPlayerButton> buttons = RosterButtonCreator.Create();
        if (buttons.Count > 0) {
          this.ToStartSelected = buttons[0];
          this.InitRosterOptions(buttons);
        }
      }
      this.BackState = MenuState.Main;
      this.TweenBGCameraToY (1);
    }

    public void Roster()
    {
      this.State = (MenuState)ROSTER;
    }

    public void DestroyRoster()
    {
    }

    public void InitRosterOptions (List<RosterPlayerButton> buttons)
    {
      for (int i = 0; i < buttons.Count; i++) {
        RosterPlayerButton optionsButton = buttons [i];
        optionsButton.TweenTo = new Vector2 (250f, (float)(45 + i * 15));
        optionsButton.Position = (optionsButton.TweenFrom = new Vector2 ((float)((i % 2 == 0) ? (-160) : 580), (float)(45 + i * 12)));
        if (i > 0) {
          optionsButton.UpItem = buttons [i - 1];
        }
        if (i < buttons.Count - 1) {
          optionsButton.DownItem = buttons [i + 1];
        }
        this.Layers [optionsButton.LayerIndex].Add(optionsButton, false);
      }
    }
  }
}