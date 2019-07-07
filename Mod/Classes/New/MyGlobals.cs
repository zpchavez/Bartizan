namespace Mod
{
  public class MyGlobals
  {
    public static string unassignedPlayerName = "???";

    public static string[] playerNames = {
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
      unassignedPlayerName,
    };

    public static int MaxPlayers() {
      return 8;
    }
  }
}
