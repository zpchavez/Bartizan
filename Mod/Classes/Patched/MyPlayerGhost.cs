using Microsoft.Xna.Framework;
using Patcher;
using TowerFall;
using Monocle;
using System;
using New;
using System.Collections.Generic;

namespace Mod
{
  [Patch]
  public class MyPlayerGhost : PlayerGhost
  {
        public PlayerCorpse corpse;
        public bool HasSpeedBoots;
        public bool Invisible;
        public bool dodging;
        public PlayerGhostShield shield;
        public WrapHitbox shieldHitbox;
        public Counter shieldRegenCounter;
        public float InvisOpacity = 1f;

        public Scheduler scheduler;

        public MyPlayerGhost(PlayerCorpse corpse)
          : base(corpse)
        {
            this.corpse = corpse;
            this.Allegiance = corpse.Allegiance;
            this.shield = new PlayerGhostShield(this);
            this.shieldHitbox = new WrapHitbox(16f, 18f, -8f, -10f);
            base.Add(this.shield);
            this.shieldRegenCounter = new Counter(240);
        }

        public override void Die(int killerIndex, Arrow arrow, Explosion explosion, ShockCircle circle)
        {
          base.Die(killerIndex, arrow, explosion, circle);
          ((MyPlayerCorpse)(this.corpse)).hasGhost = false;

          List<Entity> players = Level.Session.CurrentLevel[GameTags.Player];
          for (int i = 0; i < players.Count; i++)
          {
            MyPlayer player = (MyPlayer)players[i];
              if (player.PlayerIndex == this.PlayerIndex)
              {
                i = players.Count;
              }
          }

          var mobLogic = this.Level.Session.RoundLogic as MobRoundLogic;
          if (mobLogic != null) {
            // Ghosts treated as players in crawl mode
            mobLogic.OnPlayerDeath(
              null, this.corpse, this.PlayerIndex, DeathCause.Arrow, // FIXME
              this.Position, killerIndex
            );
          }
          if (((MyMatchVariants)base.Level.Session.MatchSettings.Variants).GottaBustGhosts) {
               ((MySession)base.Level.Session).OnPlayerGhostDeath(this, this.corpse);
          }
        }

        public override void OnPlayerGhostCollide(PlayerGhost ghost)
        {
            if (((MyMatchVariants)base.Level.Session.MatchSettings.Variants).GhostJoust)
            {
                if (this.dodging && (base.Allegiance == Allegiance.Neutral || ghost.Allegiance != base.Allegiance))
                {
                    Vector2 value = Calc.SafeNormalize (ghost.Position - base.Position);
                    if (((MyPlayerGhost)ghost).dodging)
                    {
                        if (this.HasSpeedBoots && !((MyPlayerGhost)ghost).HasSpeedBoots)
                        {
                        }
                        else
                        {
                            this.Hurt (-value * 4f, 1, ghost.PlayerIndex, null, null, null);
                        }
                    }
                    ghost.Hurt (value * 4f, 1, this.PlayerIndex, null, null, null);
                }
                else
                {
                    base.OnPlayerGhostCollide(ghost);
                }
            }
            else
            {
                base.OnPlayerGhostCollide(ghost);
            }
        }

        public override bool OnArrowHit(Arrow arrow)
        {
            if (base.State == 0)
            {
                return false;
            }
            if ((bool)base.Level.Session.MatchSettings.Variants.NoFriendlyFire && ((base.Allegiance != Allegiance.Neutral && arrow.Allegiance == base.Allegiance) || arrow.PlayerIndex == this.PlayerIndex))
            {
                return false;
            }
            if (this.HasShield)
            {
                this.HasShield = false;
                this.Speed = arrow.Speed;
                arrow.EnterFallMode(true, false, true);
                if (arrow.PlayerIndex != -1)
                {
                    base.Level.Session.MatchStats[arrow.PlayerIndex].ShieldsBroken += 1u;
                }
                return false;
            }
            else
            {
                return base.OnArrowHit(arrow);
            }
        }

        public override void OnPlayerBounce(Player player)
        {
            if (base.State != 0)
            {
                if (base.Allegiance != Allegiance.Neutral && player.Allegiance == base.Allegiance)
                {
                    base.Speed.Y = 3f;
                    this.sprite.Scale.X = 1.5f;
                    this.sprite.Scale.Y = 0.5f;
                }
                else
                {
                    if (this.HasShield)
                    {
                        this.HasShield = false;
                        base.Speed.Y = 3f;
                        this.sprite.Scale.X = 1.5f;
                        this.sprite.Scale.Y = 0.5f;
                        base.Level.Session.MatchStats[player.PlayerIndex].ShieldsBroken += 1u;
                    }
                    else
                    {
                        base.OnPlayerBounce(player);
                    }
                }
            }
        }

        public override void Hurt (Vector2 force, int damage, int killerIndex, Arrow arrow = null, Explosion explosion = null, ShockCircle shock = null)
        {
            if (shock && killerIndex == this.PlayerIndex) {
            // ShockCircle shouldn't kill friendly ghosts
                return;
            }

            if (this.HasShield)
            {
                this.HasShield = false;
                if (explosion && explosion.PlayerIndex != -1)
                {
                    base.Level.Session.MatchStats[explosion.PlayerIndex].ShieldsBroken += 1u;
                }
            }
            else
            {
                if (this.Alive && this.CanHurt)
                {
                    this.Health -= damage;
                    if (this.Health <= 0)
                    {
                        this.Die(killerIndex, arrow, explosion, shock);
                    }
                }
            }
            this.Speed = force;
        }

        public override void Added()
        {
          base.Added();

          ((MyPlayerCorpse)(this.corpse)).spawningGhost = false;
          ((MyPlayerCorpse)(this.corpse)).hasGhost = true;

          List<Entity> players = Level.Session.CurrentLevel[GameTags.Player];
          for (int i = 0; i < players.Count; i++)
          {
            MyPlayer player = (MyPlayer)players[i];
              if (player.PlayerIndex == this.PlayerIndex)
              {
                player.spawningGhost = false;
                i = players.Count;
              }
          }
        }

        public override void DodgeEnter ()
        {
            this.dodging = true;
            base.DodgeEnter();
        }

        public override void DodgeLeave ()
        {
            this.dodging = false;
            base.DodgeLeave();
        }

        public override void Update()
        {
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).RegeneratingShields[this.PlayerIndex])
            {
                if (this.HasShield)
                {
                    this.shieldRegenCounter.Set(240);
                }
                else
                {
                    this.shieldRegenCounter.Update();
                    if (!(bool)this.shieldRegenCounter)
                    {
                        this.HasShield = true;
                    }
                }
            }

            if (this.Invisible)
            {
                this.InvisOpacity = Math.Max(this.InvisOpacity - 0.02f * Engine.TimeMult, 0.2f);
            }
            else
            {
                this.InvisOpacity = Math.Min(this.InvisOpacity + 0.05f * Engine.TimeMult, 1f);
            }

            float ghostSpeed = 1f;
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).FastGhosts)
            {
                ghostSpeed *= 1.5f;
            }
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).GhostItems && this.HasSpeedBoots)
            {
                ghostSpeed *= 1.5f;
            }

            if (ghostSpeed > 1f)
            {
                typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult * ghostSpeed, null);
                base.Update();
                typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult / ghostSpeed, null);
            } else {
                base.Update();
            }
        }

        public bool HasShield
        {
            get
            {
                return this.shield.Visible;
            }
            set
            {
                if (this.shield.Visible != value)
                {
                    if (value)
                    {
                        base.TargetCollider = this.shieldHitbox;
                        this.shield.Gain();
                    }
                    else
                    {
                        base.TargetCollider = null;
                        this.shield.Lose();
                        base.Flash(30, null);
                        TFGame.PlayerInputs[this.PlayerIndex].Rumble(0.5f, 20);
                    }
                }
            }
        }

        public override void Render()
        {
            base.Render();
            this.sprite.Color = this.blendColor * (0.9f + this.alphaSine.Value * 0.1f) * this.InvisOpacity;
        }
    }
}
