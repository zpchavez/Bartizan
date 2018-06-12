using System;
using Mod;
using TowerFall;

namespace New
{
	public struct PlayerGhostInventory
    {
		public static readonly PlayerGhostInventory Default = new PlayerGhostInventory(false, false, false);

        public bool Shield;

        public bool SpeedBoots;

        public bool Invisible;

		public PlayerGhostInventory(bool shield, bool speedBoots, bool invisible)
        {
            this.Shield = shield;
            this.SpeedBoots = speedBoots;
            this.Invisible = invisible;
        }

		public PlayerGhostInventory(MyPlayerGhost ghost)
		{
            TFGame.Log(new Exception("ghost inventory"), false);
			this = new PlayerGhostInventory(ghost.HasShield, ghost.HasSpeedBoots, ghost.Invisible);
        }

		public PlayerGhostInventory Clone()
        {
            return this;
        }
    }
}
