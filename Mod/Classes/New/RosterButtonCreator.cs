using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mod
{
  class RosterButtonCreator
  {
    public static List<RosterPlayerButton> Create(TrackerApiClient trackerClient)
    {
      List<RosterPlayerButton> buttons = new List<RosterPlayerButton> ();

      foreach (JObject player in MyGlobals.roster)
      {
        if (player.Value<bool>("active")) {
          RosterPlayerButton button = new RosterPlayerButton(player, trackerClient);
          buttons.Add (button);

          player.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "color") {
              button.SetValueFromColor(player["color"]);
            }
          };
        }
      }

      return buttons;
    }
  }
}