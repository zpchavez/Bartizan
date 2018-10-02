using Microsoft.Xna.Framework;
using TowerFall;
using Patcher;

namespace Mod
{
	[Patch]
	public class MyTreasureChest : TreasureChest
	{
		public MyTreasureChest(Vector2 position, Types graphic, AppearModes mode, Pickups pickup, int timer = 0) : base(position, graphic, mode, pickup, timer) 
		{
			
		}

		public MyTreasureChest(Vector2 position, Types graphic, AppearModes mode, Pickups[] pickups, int timer = 0) : base(position, graphic, mode, pickups, timer)
        {
        }

		public override void OnPlayerGhostCollide(PlayerGhost ghost)
        {
			
            if (!base.Flashing && this.type != Types.Large && this.type != Types.Bottomless)
            {
				if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems)
				{
					MyPlayerGhost g = (MyPlayerGhost)ghost;
					if (this.pickups[0].ToString() == "SpeedBoots" && !g.HasSpeedBoots ||
					    this.pickups[0].ToString() == "Shield" && !g.HasShield ||
					    this.pickups[0].ToString().Contains("Orb"))
					{
						this.OpenChest(ghost.PlayerIndex);
					} else
                    {
                        this.OpenChestForceBomb(ghost.PlayerIndex);
                    }
				}
				else
				{
					this.OpenChestForceBomb(ghost.PlayerIndex);
				}
					
                TFGame.PlayerInputs[ghost.PlayerIndex].Rumble(0.5f, 12);
            }
        }
    }
}
