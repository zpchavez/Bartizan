using Patcher;
using TowerFall;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Mod
{
  public class MobRoundLogic : RespawnRoundLogic
  {
    public new const Modes Mode = (Modes)43;

    PlayerGhost[] activeGhosts = new PlayerGhost[8];

    public MobRoundLogic(Session session)
      : base(session)
    {
    }

    protected override void AfterOnPlayerDeath(Player player)
    {
    }

    void RemoveGhostAndRespawn(int playerIndex, Vector2 position=default(Vector2))
    {
      if (activeGhosts[playerIndex] != null) {
        var ghost = activeGhosts[playerIndex];
        var player = this.RespawnPlayer(playerIndex);
        // if we've been given a position, make sure the ghost spawns at that position and
        // retains its speed pre-spawn.
        if (position != default(Vector2)) {
          player.Position.X = position.X;
          player.Position.Y = position.Y;

          player.Speed.X = ghost.Speed.X;
          player.Speed.Y = ghost.Speed.Y;
        }
        activeGhosts[playerIndex].RemoveSelf();
        activeGhosts[playerIndex] = null;
      }
    }

    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
    {
      base.OnPlayerDeath(player, corpse, playerIndex, cause, position, killerIndex);
      this.Session.CurrentLevel.Add(activeGhosts[playerIndex] = new PlayerGhost(corpse));

      if (killerIndex == playerIndex || killerIndex == -1) {
        if (this.Session.CurrentLevel.LivingPlayers == 0) {
          var otherPlayers = TFGame.Players.Select((playing, idx) => playing && idx != playerIndex ? (int?)idx : null).Where(idx => idx != null).ToList();
          var randomPlayer = new Random().Choose(otherPlayers).Value;
          RemoveGhostAndRespawn(randomPlayer);
        }
      } else {
        RemoveGhostAndRespawn(killerIndex, position);
      }
    }
  }
}
