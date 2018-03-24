using System;

namespace Mod
{
  public class TrackerMatchStats
  {
    public int rounds;
    public int[] kills = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] deaths = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    public int[] wins = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

    public override string ToString()
    {
      string json = "{\"api_token\": \"12345\",\"rounds\": " + rounds.ToString();
      json += ",kills: {";
      for (int index = 0; index < kills.Length; index++) {
        if (kills[index] > -1) {
          json += "\"" + ((ArcherColor)index).ToString() + "\": " + kills[index].ToString();
        }
      }
      return json;
    }
  }
}
