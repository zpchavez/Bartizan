using Microsoft.Xna.Framework;
using Patcher;
using TowerFall;
using Monocle;
using System;
using New;

namespace Mod
{
  [Patch]
  public class MyPlayerGhost : PlayerGhost
  {
        PlayerCorpse corpse;
		public bool HasSpeedBoots;
		public bool Invisible;
		public PlayerGhostShield shield;
		public WrapHitbox shieldHitbox;
		public Counter shieldRegenCounter;

		public Scheduler scheduler;

        public MyPlayerGhost(PlayerCorpse corpse)
          : base(corpse)
        {
    		this.corpse = corpse;
			this.shield = new PlayerGhostShield(this);
        }

    	public MyPlayerGhost(PlayerCorpse corpse, PlayerGhostInventory inventory)
            : base(corpse)
		{
            TFGame.Log(new Exception("new ghost with inventory"), false);
            this.corpse = corpse;

			this.shieldHitbox = new WrapHitbox(16f, 18f, -8f, -10f);
            this.shieldRegenCounter = new Counter(240);
			this.shield = new PlayerGhostShield(this);

    		if (inventory.Shield)
            {
                this.scheduler.ScheduleAction(delegate {
                    this.HasShield = true;
                }, 20 + 10 * this.PlayerIndex, false);
            }
            if (inventory.Invisible)
            {
                this.scheduler.ScheduleAction(delegate {
                    this.Invisible = true;
                }, 60, false);
            }
            if (inventory.SpeedBoots)
            {
                this.HasSpeedBoots = true;
            }

        }

        public override void Die(int killerIndex, Arrow arrow, Explosion explosion, ShockCircle circle)
        {
          base.Die(killerIndex, arrow, explosion, circle);
          var mobLogic = this.Level.Session.RoundLogic as MobRoundLogic;
          if (mobLogic != null) {
            // Ghosts treated as players in crawl mode
            mobLogic.OnPlayerDeath(
              null, this.corpse, this.PlayerIndex, DeathCause.Arrow, // FIXME
              this.Position, killerIndex
            );
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
                TFGame.Log(new Exception("ghost lost shield from arrow"), false);
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
                        TFGame.Log(new Exception("ghost lost shield on bounce"), false);
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
                TFGame.Log(new Exception("ghost lost shield via explosion"), false);
                this.HasShield = false;
                if (explosion && explosion.PlayerIndex != -1)
                {
                    base.Level.Session.MatchStats[explosion.PlayerIndex].ShieldsBroken += 1u;
                }
            }
            this.Speed = force;
                if (this.Alive && this.CanHurt) {
                this.Health -= damage;
                if (this.Health <= 0) {
                    this.Die (killerIndex, arrow, explosion, shock);
                }
            }
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
            if (((MyMatchVariants)Level.Session.MatchSettings.Variants).FastGhosts) {
                typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult * 1.5f, null);
                base.Update();
                typeof(Engine).GetProperty("TimeMult").SetValue(null, Engine.TimeMult / 1.5f, null);
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
                        //base.TargetCollider = this.shieldHitbox;
						this.shield.Gain();
                        TFGame.Log(new Exception("ghost has shield"), false);
                    }
                    else
                    {
                        //base.TargetCollider = null;
                        this.shield.Lose();
                        //base.Flash(30, null);
                        //TFGame.PlayerInputs[this.PlayerIndex].Rumble(0.5f, 20);
                    }
                }
            }
        }
	}
}
