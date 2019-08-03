using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mod
{
  class RosterButtonCreator
  {
    public static List<RosterPlayerButton> Create()
    {
      List<RosterPlayerButton> buttons = new List<RosterPlayerButton> ();

      foreach (JObject player in MyGlobals.roster)
      {
        if (player.Value<bool>("active")) {
          RosterPlayerButton button = new RosterPlayerButton(player);
          buttons.Add (button);
        }
      }

      return buttons;
    }
  }
}