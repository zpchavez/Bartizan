using TowerFall;
using Patcher;
using System.IO;

namespace Mod
{
	[Patch]
	public class MyMapScene : MapScene
    {
		private static bool initialLoad = true;
		      
		public MyMapScene(MainMenu.RollcallModes mode) : base(mode) 
		{
		}

		static MyMapScene()
        {
            MyMapScene.lastRandomVersusTower = -1;
            MyMapScene.NoRandomStates = new bool[GameData.VersusTowers.Count];
        }
        
		public override void InitVersusButtons()
		{			         
			this.Buttons.Add(new VersusRandomSelect());
            string[] defaultMaps;

			for(int i = 0; i < GameData.VersusTowers.Count; i++) {
                if (SaveData.Instance.Unlocks.GetTowerUnlocked(i))
                {
                    this.Buttons.Add(new VersusMapButton(GameData.VersusTowers[i]));
                }
			}

			string defaultMapsFile = Path.Combine(MyVersusMatchResults.GetSavePath(), "tf_disabled_maps.txt");
            if (initialLoad && File.Exists(defaultMapsFile))
            {
				initialLoad = false;
                defaultMaps = File.ReadAllLines(defaultMapsFile);

				if (defaultMaps.Length != 0)
				{
					for (int i = 0; i < defaultMaps.Length; i++)
					{                
						for (int k = 0; k < this.Buttons.Count; k++) 
						{
							if (this.Buttons[k].Title.Equals(defaultMaps[i]))
							{
								if (!((VersusMapButton)this.Buttons[k]).NoRandom)
                                {
									this.Buttons[k].AltAction();
								}
							}
						}
                    }
                }
			}

            this.LinkButtonsList();
            if (base.HasBegun)
            {
                this.InitButtons(this.Buttons[2]);
                
				for (int j = 0; j < this.Buttons.Count; j++)
                {
					base.Layers[this.Buttons[j].LayerIndex].Add(this.Buttons[j], false);
                }
            }
        }
    }
}
