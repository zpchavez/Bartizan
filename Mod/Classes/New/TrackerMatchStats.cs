using TowerFall;
using System;

namespace Mod
{
  public class TrackerMatchStats
  {
    public int rounds;
    public int[] kills = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] deaths = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] wins = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] selfs = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] teamKills = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] revives = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] killsAsGhost = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] ghostKills = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] miracles = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

    public string FieldToJSON(string field)
    {
      int[] stat;
      switch(field) {
        case "kills":
          stat = kills;
          break;
        case "deaths":
          stat = deaths;
          break;
        case "wins":
          stat = wins;
          break;
        case "selfs":
          stat = selfs;
          break;
        case "team_kills":
          stat = teamKills;
          break;
        case "revives":
          stat = revives;
          break;
        case "killsAsGhost":
          stat = revives;
          break;
        case "ghostKills":
          stat = revives;
          break;
        case "miracles":
          stat = revives;
          break;
        default:
          throw new Exception("Invalid field: " + field);
      }

      string jsonPart = ",\"" + field + "\": {";

      string[] stringParts = new string[TFGame.PlayerAmount];
      int counter = 0;
      for (int index = 0; index < stat.Length; index++) {
        if (stat[index] > -1) {
          stringParts[counter] = "\"" + ((ArcherColor)TFGame.Characters[index]).ToString() + "\": " + stat[index].ToString();
          counter++;
        }
      }
      jsonPart += String.Join(",", stringParts) + "}";
      return jsonPart;
    }

    public string ToJSON(string apiKey)
    {
      string json = "{\"api_token\": \"" + apiKey + "\",\"rounds\": " + rounds.ToString();

      json += FieldToJSON("kills");
      json += FieldToJSON("deaths");
      json += FieldToJSON("wins");
      json += FieldToJSON("selfs");
      json += FieldToJSON("team_kills");
      json += FieldToJSON("revives");
      json += FieldToJSON("killsAsGhost");
      json += FieldToJSON("ghostKills");
      json += FieldToJSON("miracles");

      json += "}";

      return json;
    }
  }
}
