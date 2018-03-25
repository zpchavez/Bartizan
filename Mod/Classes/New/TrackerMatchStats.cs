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

    public string ToJSON(string apiKey)
    {
      string json = "{\"api_token\": \"" + apiKey + "\",\"rounds\": " + rounds.ToString();

      json += ",\"kills\": {";
      string[] killStrings = new string[TFGame.PlayerAmount];
      int counter = 0;
      for (int index = 0; index < kills.Length; index++) {
        if (kills[index] > -1) {
          killStrings[counter] = "\"" + ((ArcherColor)index).ToString() + "\": " + kills[index].ToString();
          counter++;
        }
      }
      json += String.Join(",", killStrings) + "}";

      json += ",\"deaths\": {";
      string[] deathStrings = new string[TFGame.PlayerAmount];
      counter = 0;
      for (int index = 0; index < deaths.Length; index++) {
        if (deaths[index] > -1) {
          deathStrings[counter] = "\"" + ((ArcherColor)index).ToString() + "\": " + deaths[index].ToString();
          counter++;
        }
      }
      json += String.Join(",", deathStrings) + "}";

      json += ",\"wins\": {";
      string[] winStrings = new string[TFGame.PlayerAmount];
      counter = 0;
      for (int index = 0; index < wins.Length; index++) {
        if (wins[index] > -1) {
          winStrings[counter++] = "\"" + ((ArcherColor)index).ToString() + "\": " + wins[index].ToString();
        }
      }
      json += String.Join(",", winStrings) + "}";

      json += "}";

      return json;
    }
  }
}
