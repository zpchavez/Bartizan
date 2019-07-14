using TowerFall;
using Patcher;
using System;
using System.IO;

namespace Mod
{
  [Patch]
  public class MyMapScene : MapScene
    {
    private static bool initialLoad = true;

    public MyMapScene(MainMenu.RollcallModes mode) : base(mode)
    {
      TrackerApiClient client = new TrackerApiClient();
      client.GetPlayerNames();
    }

    static MyMapScene()
    {
      MyMapScene.lastRandomVersusTower = -1;
      MyMapScene.NoRandomStates = new bool[GameData.VersusTowers.Count];
    }

    public override void InitVersusButtons()
    {
      base.InitVersusButtons();

      string disabledMapsFile = Path.Combine(TrackerApiClient.GetSavePath(), "tf-disabled-maps.txt");

      if (initialLoad && File.Exists(disabledMapsFile)) {
        initialLoad = false;
        string[] disabledMaps = File.ReadAllLines(disabledMapsFile);

        if (disabledMaps != null && disabledMaps.Length != 0) {
          for (int i = 0; i < this.Buttons.Count; i++) {
            bool isDisabled = false;
            for (int j = 0; j < disabledMaps.Length; j++) {
              if (disabledMaps[j] == this.Buttons[i].Title) {
                isDisabled = true;
                break;
              }
            }
            if (isDisabled && !((VersusMapButton)this.Buttons[i]).NoRandom) {
              this.Buttons[i].AltAction();
            }
          }
        }
      }
    }
  }
}
