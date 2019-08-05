using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using TowerFall;
using System;

namespace Mod
{
  class RosterButtonCreator
  {
    public static List<RosterPlayerButton> Create(int pageIndex, TrackerApiClient trackerClient)
    {
      List<RosterPlayerButton> buttons = new List<RosterPlayerButton> ();

      int offset = pageIndex * MyGlobals.ROSTER_PAGE_SIZE;
      bool isFullPage = MyGlobals.roster.Count >= (offset + MyGlobals.ROSTER_PAGE_SIZE);
      int stopAt;
      if (isFullPage) {
        stopAt = offset + MyGlobals.ROSTER_PAGE_SIZE;
      } else {
        stopAt = offset + (MyGlobals.roster.Count % MyGlobals.ROSTER_PAGE_SIZE);
      }

      for (
        int index = pageIndex * MyGlobals.ROSTER_PAGE_SIZE;
        index < stopAt;
        index += 1
      ) {
        JObject player = (JObject)MyGlobals.roster[index];
        RosterPlayerButton button = new RosterPlayerButton(player, trackerClient);
        buttons.Add (button);

        player.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
          if (e.PropertyName == "color") {
            button.SetValueFromColor(player["color"]);
          }
        };
      }

      return buttons;
    }
  }
}