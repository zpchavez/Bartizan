using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mod
{
  class RosterButtonCreator
  {
    public static List<MyRosterPlayerButton> Create()
    {
      List<MyRosterPlayerButton> buttons = new List<MyRosterPlayerButton> ();

      foreach (JObject player in MyGlobals.roster)
      {
        if (player.Value<bool>("active")) {
          MyRosterPlayerButton button = new MyRosterPlayerButton(player);
          buttons.Add (button);
        }
      }

      return buttons;
    }
  }
}