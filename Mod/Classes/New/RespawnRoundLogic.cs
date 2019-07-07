using TowerFall;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

namespace Mod
{
  public class RespawnRoundLogic : RoundLogic
  {
    public const Modes Mode = (Modes)42;

    private KillCountHUD[] killCountHUDs = new KillCountHUD[MyGlobals.MAX_PLAYERS];
    private bool wasFinalKill;
    private Counter endDelay;

    public RespawnRoundLogic(Session session)
      : base(session, canHaveMiasma: false)
    {
      for (int i = 0; i < MyGlobals.MAX_PLAYERS; i++) {
        if (TFGame.Players[i]) {
          killCountHUDs[i] = new KillCountHUD(i);
          this.Session.CurrentLevel.Add(killCountHUDs[i]);
        }
      }
      this.endDelay = new Counter();
      this.endDelay.Set(90);
    }

    public override void OnLevelLoadFinish()
    {
      base.OnLevelLoadFinish();
      base.Session.CurrentLevel.Add<VersusStart>(new VersusStart(base.Session));
      base.Players = base.SpawnPlayersFFA();
    }

    public override bool FFACheckForAllButOneDead()
    {
      return false;
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (base.RoundStarted && base.Session.CurrentLevel.Ending && base.Session.CurrentLevel.CanEnd) {
        if (this.endDelay) {
          this.endDelay.Update();
          return;
        }
        base.Session.EndRound();
      }
    }

    protected Player RespawnPlayer(int playerIndex)
    {
      List<Vector2> spawnPositions = this.Session.CurrentLevel.GetXMLPositions("PlayerSpawn");

      var player = new Player(playerIndex, new Random().Choose(spawnPositions), Allegiance.Neutral, Allegiance.Neutral,
                    this.Session.GetPlayerInventory(playerIndex), this.Session.GetSpawnHatState(playerIndex),
              frozen: false, flash: false, indicator: true);
      this.Session.CurrentLevel.Add(player);
      player.Flash(120, null);
      Alarm.Set(player, 60, player.RemoveIndicator, Alarm.AlarmMode.Oneshot);
      return player;
    }

    protected virtual void AfterOnPlayerDeath(Player player)
    {
      this.RespawnPlayer(player.PlayerIndex);
    }

    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
    {
      base.OnPlayerDeath(player, corpse, playerIndex, cause, position, killerIndex);

      if (killerIndex == playerIndex || killerIndex == -1) {
        killCountHUDs[playerIndex].Decrease();
        base.AddScore(playerIndex, -1);
      } else if (killerIndex != -1) {
        killCountHUDs[killerIndex].Increase();
        base.AddScore(killerIndex, 1);
      }

      int winner = base.Session.GetWinner();
      if (this.wasFinalKill && winner == -1) {
        this.wasFinalKill = false;
        base.Session.CurrentLevel.Ending = false;
        base.CancelFinalKill();
        this.endDelay.Set(90);
      }
      if (!this.wasFinalKill && winner != -1) {
        base.Session.CurrentLevel.Ending = true;
        this.wasFinalKill = true;
        base.FinalKill(corpse, winner);
      }

      this.AfterOnPlayerDeath(player);
    }
  }

  public class KillCountHUD : Entity
  {
    int playerIndex;
    List<Sprite<int>> skullIcons = new List<Sprite<int>>();

    public int Count { get { return this.skullIcons.Count; } }

    public KillCountHUD(int playerIndex)
      : base(3)
    {
      this.playerIndex = playerIndex;
    }

    public void Increase()
    {
      Sprite<int> sprite = DeathSkull.GetSprite();

      if (this.playerIndex % 2 == 0) {
        sprite.X = 8 + 10 * skullIcons.Count;
      } else {
        sprite.X = 320 - 8 - 10 * skullIcons.Count;
      }

      sprite.Y = this.playerIndex / 2 == 0 ? 20 : 240 - 20;
      //sprite.Play(0, restart: false);
      sprite.Stop();
      this.skullIcons.Add(sprite);
      base.Add(sprite);
    }

    public void Decrease()
    {
      if (this.skullIcons.Any()) {
        base.Remove(this.skullIcons.Last());
        this.skullIcons.Remove(this.skullIcons.Last());
      }
    }

    public override void Render()
    {
      foreach (Sprite<int> sprite in this.skullIcons) {
        sprite.DrawOutline(1);
      }
      base.Render();
    }
  }
}
