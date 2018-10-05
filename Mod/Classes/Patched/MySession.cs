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

    public void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse)
    {
        String logicName = this.RoundLogic.GetType().Name;
        switch (logicName)
        {
            case "TeamDeathmatchRoundLogic":
                ((MyTeamDeathmatchRoundLogic)this.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
                break;
            case "HeadhuntersRoundLogic":
                ((MyHeadhuntersRoundLogic)this.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
                break;
            case "LastManStandingRoundLogic":
                ((MyLastManStandingRoundLogic)this.RoundLogic).OnPlayerGhostDeath(ghost, corpse);
                break;
        }
    }

    public void OnTeamRevive(Player player)
    {
        String logicName = this.RoundLogic.GetType().Name;
        switch (logicName)
        {
            case "TeamDeathmatchRoundLogic":
                ((MyTeamDeathmatchRoundLogic)this.RoundLogic).OnTeamRevive(player);
                break;
        }
    }
  }
}
