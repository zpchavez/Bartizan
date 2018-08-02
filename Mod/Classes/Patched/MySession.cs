using TowerFall;
using Patcher;
using System;
using Microsoft.Xna.Framework;

namespace Mod
{
  [Patch]
  public class MySession : Session, GhostDeathInterface
  {
    public int RoundsPlayedThisMatch;

    public MySession (MatchSettings settings) : base(settings)
    {
      RoundsPlayedThisMatch = 0;
    }
    
    public override void LevelLoadStart (Level level)
    {
        this.CurrentLevel = level;
        this.RoundLogic = MyRoundLogic.GetMyRoundLogic (this);
        if (this.TreasureSpawner != null) {
            this.RoundRandomArrowType = this.TreasureSpawner.GetRandomArrowType (true);
        }
    }
    
    public override void OnPlayerDeath (Player player, PlayerCorpse corpse, int playerIndex, DeathCause deathType, Vector2 position, int killerIndex)
    {
        this.RoundLogic.OnPlayerDeath (player, corpse, playerIndex, deathType, position, killerIndex);
    }
        
    public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
    {
		if ((this.RoundLogic).GetType().Name.Contains("My"))
		{
			((GhostDeathInterface)this.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
		}
    }
  }
}
