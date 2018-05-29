using System;
using TowerFall;
using Microsoft.Xna.Framework;
using Patcher;

namespace Mod
{
  [Patch]
  public class MyPlayer : Player
  {
    public bool spawningGhost;

    private string lastHatState = "UNSET";

    MyChaliceGhost summonedChaliceGhost;

    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, Player.HatStates hatState, bool frozen, bool flash, bool indicator)
      : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
    }

    public override void Added()
    {
      base.Added();

      this.spawningGhost = false;

      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).VarietyPack[this.PlayerIndex]) {
        this.Arrows.Clear();
        this.Arrows.SetMaxArrows(10);
        var arrows = new ArrowTypes[] {
          ArrowTypes.Bomb,
          ArrowTypes.SuperBomb,
          ArrowTypes.Laser,
          ArrowTypes.Bramble,
          ArrowTypes.Drill,
          ArrowTypes.Bolt,
          ArrowTypes.Toy,
          ArrowTypes.Feather,
          ArrowTypes.Trigger,
          ArrowTypes.Prism
        };

        // Randomize. Couldn't get static method to work.
        Random rand = new Random();
        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < arrows.Length - 1; i++)
        {
          int j = rand.Next(i, arrows.Length);
          ArrowTypes temp = arrows[i];
          arrows[i] = arrows[j];
          arrows[j] = temp;
        }
        this.Arrows.AddArrows(arrows);
      }
    }

    public override bool CanGrabLedge(int a, int b)
    {
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoLedgeGrab[this.PlayerIndex])
        return false;
      return base.CanGrabLedge(a, b);
    }

    public override int GetDodgeExitState()
    {
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoDodgeCooldowns[this.PlayerIndex]) {
        this.DodgeCooldown();
      }
      return base.GetDodgeExitState();
    }

    public override void ShootArrow()
    {
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).InfiniteArrows[this.PlayerIndex]) {
        var arrow = this.Arrows.Arrows[0];
        base.ShootArrow();
        this.Arrows.AddArrows(arrow);
      } else {
        base.ShootArrow();
      }
    }

    public override void HurtBouncedOn(int bouncerIndex)
    {
      if (!((MyMatchVariants)Level.Session.MatchSettings.Variants).NoHeadBounce[this.PlayerIndex])
        base.HurtBouncedOn(bouncerIndex);
    }

    public override PlayerCorpse Die (DeathCause deathCause, int killerIndex, bool brambled = false, bool laser = false)
    {
      if (summonedChaliceGhost) {
        summonedChaliceGhost.Vanish();
        summonedChaliceGhost = null;
      }
      return base.Die(deathCause, killerIndex, brambled, laser);
    }

    public override void Update()
    {
      base.Update();
      if (((MyMatchVariants)Level.Session.MatchSettings.Variants).CrownSummonsChaliceGhost) {
        if (lastHatState == "UNSET") {
          lastHatState = HatState.ToString();
        } else if (lastHatState != HatState.ToString()) {
          if (lastHatState != "Crown" && HatState.ToString() == "Crown") {
            MyChalicePad chalicePad = new MyChalicePad(ActualPosition, 4);
            MyChalice chalice = new MyChalice(chalicePad);
            summonedChaliceGhost = new MyChaliceGhost(
              PlayerIndex,
              chalice,
              ((MyMatchVariants)Level.Session.MatchSettings.Variants).ChaliceGhostsHuntGhosts
            );
            Level.Layers[summonedChaliceGhost.LayerIndex].Add(summonedChaliceGhost, false);
          } else if (summonedChaliceGhost && lastHatState == "Crown" && HatState.ToString() != "Crown") {
            // Ghost vanishes when player loses the crown
            summonedChaliceGhost.Vanish();
            summonedChaliceGhost = null;
          }
          lastHatState = HatState.ToString();
        }
      }
    }
  }
}
