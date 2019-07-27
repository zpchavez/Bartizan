namespace Mod
{
  public class MyGlobals
  {
    public static PlayerNames playerNames;

    #if (EIGHT_PLAYER)
      public const int MAX_PLAYERS = 8;
    #else
      public const int MAX_PLAYERS = 4;
    #endif
  }
}
