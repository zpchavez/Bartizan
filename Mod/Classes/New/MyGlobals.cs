using Newtonsoft.Json.Linq;

namespace Mod
{
  public class MyGlobals
  {
    public static int ROSTER_PAGE_SIZE = 10;

    public static PlayerNames playerNames;

    public static JArray roster;

    #if (EIGHT_PLAYER)
      public const int MAX_PLAYERS = 8;
    #else
      public const int MAX_PLAYERS = 4;
    #endif
  }
}
