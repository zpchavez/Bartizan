using System;
using TowerFall;
using Microsoft.Xna.Framework;
using Patcher;
using Monocle;

namespace Mod
{
  [Patch]
  public class MyPlayer : Player
  {
    private string lastHatState = "UNSET";
	public bool spawningGhost;
	public bool diedFromPrism = false;

    MyChaliceGhost summonedChaliceGhost;

    public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, Player.HatStates hatState, bool frozen, bool flash, bool indicator)
      : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
    {
    }

    public override void Added()
    {
      base.Added();
	  this.spawningGhost = false;
      this.diedFromPrism = false;
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

    public override void HotCoalsBounce ()
    {
		if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce)
		{
            this.Fire.Start ();
			if (this.Speed.Y >= 0f) {
                if (this.input.JumpCheck) {
                    Sounds.sfx_coalBurn.Play (base.X, 1f);
                }
            }
		}
		else
		{
            base.HotCoalsBounce();
		}
    }
    
    public override void Die (Arrow arrow)
    {
        Vector2 value = Calc.SafeNormalize (arrow.Speed);
        int ledge = (int)((this.state.PreviousState == 1 && Vector2.Dot (Vector2.UnitX * (float)this.Facing, value) > 0.8f) ? this.Facing : ((Facing)0));
        int playerIndex = arrow.PlayerIndex;
        if (playerIndex == this.PlayerIndex && arrow is LaserArrow) {
            base.Level.Session.MatchStats [this.PlayerIndex].SelfLaserKills += 1u;
        }
        if (arrow.State == Arrow.ArrowStates.Falling && arrow.PlayerIndex != -1 && arrow.PlayerIndex != this.PlayerIndex) {
            base.Level.Session.MatchStats [arrow.PlayerIndex].DroppedArrowKills += 1u;
        }
        if (arrow.FromHyper) {
            if (playerIndex == this.PlayerIndex) {
                base.Level.Session.MatchStats [this.PlayerIndex].HyperSelfKills += 1u;
            } else {
                base.Level.Session.MatchStats [arrow.PlayerIndex].HyperArrowKills += 1u;
            }
        }
		this.diedFromPrism = arrow is PrismArrow;
        
        this.Die (DeathCause.Arrow, playerIndex, arrow is BrambleArrow, arrow is LaserArrow).DieByArrow (arrow, ledge);
    }

    public override PlayerCorpse Die (DeathCause deathCause, int killerIndex, bool brambled = false, bool laser = false)
    {
        if (summonedChaliceGhost) {
            summonedChaliceGhost.Vanish();
            summonedChaliceGhost = null;
        }
      
        if (Level.Session.MatchSettings.Variants.ReturnAsGhosts[this.PlayerIndex] && !this.diedFromPrism)
        {
            this.spawningGhost = true;
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
    public override int NormalUpdate ()
    {
        if (this.OnGround && (bool)this.inMud && this.Speed.X != 0f && base.Level.OnInterval (10)) {
            Sounds.env_mudPlayerMove.Play (base.X, 1f);
        }
		if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce)
		{
			if (this.InputDucking)
            {
                if (!this.startAimingDown)
                {
                    return 2;
                }
            }
            else if (this.startAimingDown)
            {
                this.startAimingDown = false;
            }
		}
		else
		{
			if (!this.onHotCoals && this.InputDucking)
			{
				if (!this.startAimingDown)
				{
					return 2;
				}
			}
			else if (this.startAimingDown)
			{
				this.startAimingDown = false;
			}
		}
        float num = MathHelper.Lerp (0.35f, 1f, this.slipperyControl);
        if (this.inMud != null && this.OnGround && this.moveAxis.X != (float)Math.Sign (this.Speed.X)) {
            this.Speed.X = Calc.Approach (this.Speed.X, 0f, 0.1f);
        } else if ((this.Aiming && this.OnGround) || (!this.Aiming && this.slipperyControl == 1f && this.moveAxis.X != (float)Math.Sign (this.Speed.X))) {
            float maxMove = (!this.HasWings) ? (((this.OnGround || this.HasWings) ? 0.2f : 0.14f) * num * Engine.TimeMult) : (((Math.Abs (this.Speed.X) > this.MaxRunSpeed) ? 0.14f : 0.2f) * num * Engine.TimeMult);
            this.Speed.X = Calc.Approach (this.Speed.X, 0f, maxMove);
        }
        if (!this.Aiming && this.moveAxis.X != 0f) {
            if (this.OnGround && num == 1f && this.inMud == null) {
                if (Math.Sign (this.moveAxis.X) == -Math.Sign (this.Speed.X) && base.Level.OnInterval (1)) {
                    base.Level.Particles.Emit (this.DustParticleType, 2, base.Position + new Vector2 ((float)(-4 * Math.Sign (this.moveAxis.X)), 6f), Vector2.One * 2f);
                } else if (this.moveAxis.X != 0f && this.HasSpeedBoots && Math.Abs (this.Speed.X) >= 1.5f && base.Level.OnInterval (2)) {
                    base.Level.Particles.Emit (this.DustParticleType, 1, base.Position + new Vector2 ((float)(-4 * Math.Sign (this.moveAxis.X)), 6f), Vector2.One * 2f);
                }
            }
            if (Math.Abs (this.Speed.X) > this.MaxRunSpeed && (float)Math.Sign (this.Speed.X) == this.moveAxis.X) {
                this.Speed.X = Calc.Approach (this.Speed.X, this.MaxRunSpeed * this.moveAxis.X, 0.03f * Engine.TimeMult);
            } else {
                float num2 = this.OnGround ? 0.15f : 0.1f;
                num2 *= num;
                if (this.dodgeCooldown) {
                    num2 *= 0.8f;
                }
                if (this.HasSpeedBoots) {
                    num2 *= 1.4f;
                }
                this.Speed.X = Calc.Approach (this.Speed.X, this.MaxRunSpeed * this.moveAxis.X, num2 * Engine.TimeMult);
            }
        }
        if (this.Speed.Y < -3.2f) {
            if (this.canPadParticles && base.Level.OnInterval (1)) {
                base.Level.Particles.Emit (Particles.JumpPadTrail, Calc.Range (Calc.Random, base.Position, Vector2.One * 4f));
            }
        } else {
            this.canPadParticles = false;
        }
        this.Cling = 0;
        if (this.OnGround) {
            this.wings.Normal ();
        } else {
            this.flapGravity = Calc.Approach (this.flapGravity, 1f, ((this.flapGravity < 0.5f) ? 0.012f : 0.048f) * Engine.TimeMult);
            if (this.autoBounce && this.Speed.Y > 0f) {
                this.autoBounce = false;
            }
            float num3 = (this.Speed.Y <= 1f && (this.input.JumpCheck || this.autoBounce) && this.canVarJump) ? 0.15f : 0.3f;
            num3 *= this.flapGravity;
            float target = 2.8f;
            if (this.moveAxis.X != 0f && this.CanWallSlide ((Facing)(int)this.moveAxis.X)) {
                this.wings.Normal ();
                target = this.wallStickMax;
                this.wallStickMax = Calc.Approach (this.wallStickMax, 1.6f, 0.01f * Engine.TimeMult);
                this.Cling = (int)this.moveAxis.X;
                if (this.Speed.Y > 0f) {
                    this.ArcherData.SFX.WallSlide.Play (base.X, 1f);
                }
                if (base.Level.OnInterval (3)) {
                    base.Level.Particles.Emit (this.DustParticleType, 1, base.Position + new Vector2 ((float)(3 * this.Cling), 0f), new Vector2 (1f, 3f));
                }
            } else if (this.input.MoveY == 1 && this.Speed.Y > 0f) {
                this.wings.FallFast ();
                target = 3.5f;
                base.Level.Session.MatchStats [this.PlayerIndex].FastFallFrames += Engine.TimeMult;
            } else if (this.input.JumpCheck && this.HasWings && this.Speed.Y >= -1f) {
                this.wings.Glide ();
                this.gliding = true;
                target = 0.8f;
            } else {
                this.wings.Normal ();
            }
            if (this.Cling == 0 || this.Speed.Y <= 0f) {
                this.ArcherData.SFX.WallSlide.Stop (true);
            }
            this.Speed.Y = Calc.Approach (this.Speed.Y, target, num3 * Engine.TimeMult);
        }
        if (!this.dodgeCooldown && this.input.DodgePressed && !base.Level.Session.MatchSettings.Variants.NoDodging [this.PlayerIndex]) {
            if (this.moveAxis.X != 0f) {
                this.Facing = (Facing)(int)this.moveAxis.X;
            }
            return 3;
        }
		if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce)
		{
			if (this.onHotCoals)
			{
				this.HotCoalsBounce();
			}
			if (this.input.JumpPressed || (bool)this.jumpBufferCounter)
			{
				if ((bool)this.jumpGraceCounter)
				{
					int num4 = this.graceLedgeDir;
					if (this.input.MoveX != num4)
					{
						num4 = 0;
					}
					this.Jump(true, true, false, num4, false);
				}
				else if (this.CanWallJump(Facing.Left))
				{
					this.WallJump(1);
				}
				else if (this.CanWallJump(Facing.Right))
				{
					this.WallJump(-1);
				}
				else if (this.canDoubleJump)
				{
					this.canDoubleJump = false;
					this.Jump(true, false, false, 0, true);
				}
				else if (this.HasWings && !(bool)this.flapBounceCounter)
				{
					this.WingsJump();
				}
			}
		}
		else
		{
            if (this.onHotCoals)
            {
                this.HotCoalsBounce();
            }
            else if (this.input.JumpPressed || (bool)this.jumpBufferCounter)
            {
                if ((bool)this.jumpGraceCounter)
                {
                    int num4 = this.graceLedgeDir;
                    if (this.input.MoveX != num4)
                    {
                        num4 = 0;
                    }
                    this.Jump(true, true, false, num4, false);
                }
                else if (this.CanWallJump(Facing.Left))
                {
                    this.WallJump(1);
                }
                else if (this.CanWallJump(Facing.Right))
                {
                    this.WallJump(-1);
                }
                else if (this.canDoubleJump)
                {
                    this.canDoubleJump = false;
                    this.Jump(true, false, false, 0, true);
                }
                else if (this.HasWings && !(bool)this.flapBounceCounter)
                {
                    this.WingsJump();
                }
            }
		}
        if (!Player.ShootLock) {
            if (this.triggerArrows.Count > 0) {
                if (this.input.AltShootPressed) {
                    this.DetonateTriggerArrows ();
                }
                if (this.Aiming && !this.input.ShootCheck) {
                    this.ShootArrow ();
                } else if (!this.Aiming && this.input.ShootPressed) {
                    this.Aiming = true;
                }
            } else if (this.Aiming) {
                if (!this.input.AltShootCheck && !this.input.ShootCheck) {
                    this.ShootArrow ();
                }
            } else if (this.didDetonate) {
                if (!this.input.AltShootCheck) {
                    this.didDetonate = false;
                }
                if (this.input.ShootPressed) {
                    this.Aiming = true;
                }
            } else if (this.input.AltShootPressed || this.input.ShootPressed) {
                this.Aiming = true;
            }
        }
        float num5 = 0f;
        Entity entity = null;
        bool flag = this.Speed.Y < -1f && this.Speed.Y > -3.2f && this.input.JumpCheck && (entity = base.CollideFirst (GameTags.JumpThru)) != null;
        if (flag) {
            num5 = -2f;
            (entity as JumpThru).OnPlayerPop (base.X);
            if (!this.didPopThroughJumpThru) {
                Sounds.env_gplatPlayerJump.Play (base.X, 1f);
            }
        }
        this.didPopThroughJumpThru = flag;
        if (this.moveAxis.X != 0f) {
            this.Facing = (Facing)(int)this.moveAxis.X;
        }
        base.MoveH (this.Speed.X * Engine.TimeMult, this.onCollideH);
        base.MoveV ((this.Speed.Y + num5) * Engine.TimeMult, this.onCollideV);
        if (this.Prism == null && !this.OnGround && !this.Aiming && this.Speed.Y >= 0f && this.moveAxis.X != 0f && this.moveAxis.Y <= 0f && base.CollideCheck (GameTags.Solid, base.Position + Vector2.UnitX * this.moveAxis.X * 2f)) {
            int direction = Math.Sign (this.moveAxis.X);
            for (int i = 0; i < 10; i++) {
                if (this.CanGrabLedge ((int)base.Y - i, direction)) {
                    return this.GrabLedge ((int)base.Y - i, direction);
                }
            }
        }
        return 0;
    }

    public override int DodgingUpdate ()
    {
        this.InvisOpacity = 1f;
        this.wings.Normal ();
        Entity entity = default(Entity);
        if (this.Speed.Y < -0.5f && (entity = base.CollideFirst (GameTags.JumpThru)) != null) {
            base.MoveV (-2f * Engine.TimeMult, null);
            (entity as JumpThru).OnPlayerPop (base.X);
        } else if (this.Speed.Y == 0f && base.CollideCheck (GameTags.JumpThru) && !base.CollideCheck (GameTags.JumpThru, base.Position + Vector2.UnitY * -5f)) {
            base.MoveV (-1f * Engine.TimeMult, null);
        }
        if (base.Level.OnInterval (1)) {
            base.Level.Particles.Emit (this.DashParticleType, 2, base.Position, Vector2.One * 4f);
            if (this.OnGround && this.DodgeSliding && this.inMud == null) {
                base.Level.Particles.Emit (this.DustParticleType, 2, base.Position + new Vector2 (0f, 8f), new Vector2 (4f, 1f));
            }
        }
        if (this.triggerArrows.Count > 0 && this.input.ShootPressed) {
            this.DetonateTriggerArrows ();
        }
        if (this.onHotCoals && !((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce && (!this.DodgeSliding || this.CanUnduck ())) {
            this.HotCoalsBounce ();
            return 0;
        }
        if (this.DodgeSliding) {
            this.DoHatPickupCheck ();
        }
        if (!(bool)this.dodgeCatchCounter) {
            this.bodySprite.Scale.X = Calc.Approach (this.bodySprite.Scale.X, 1f, 0.02f * Engine.TimeMult);
            this.bodySprite.Scale.Y = Calc.Approach (this.bodySprite.Scale.Y, 1f, 0.02f * Engine.TimeMult);
            if (this.Speed.X != 0f) {
                if (Math.Abs (this.Speed.X) > 0.02f && base.CollideCheck (GameTags.Solid, base.Position + Vector2.UnitX * (float)Math.Sign (this.Speed.X))) {
                    this.OnCollideHDodging (null);
                }
                base.MoveH (this.Speed.X * Engine.TimeMult, this.onDodgeCollideH);
            }
            if (this.Speed.Y > 0f) {
                if (this.Speed.Y > 0.02f && (base.CollideCheck (GameTags.Solid, base.Position + Vector2.UnitY) || base.CollideCheckOutside (GameTags.JumpThru, base.Position + Vector2.UnitY))) {
                    this.OnCollideVDodging (null);
                }
                base.MoveV (this.Speed.Y * Engine.TimeMult, this.onDodgeCollideV);
            } else if (this.Speed.Y < 0f) {
                if (this.Speed.Y < -0.02f && base.CollideCheck (GameTags.Solid, base.Position - Vector2.UnitY)) {
                    this.OnCollideVDodging (null);
                }
                base.MoveV (this.Speed.Y * Engine.TimeMult, this.onDodgeCollideV);
            }
        } else {
            this.dodgeCatchCounter.Update ();
        }
        this.Speed *= (float)Math.Pow (0.85000002384185791, (double)Engine.TimeMult);
        Collider collider = base.Collider;
        base.Collider = this.dodgeCatchHitbox;
        Arrow arrow = base.CollideFirst (GameTags.Arrow) as Arrow;
        if (arrow != null && (!this.HasShield || !arrow.Dangerous)) {
            this.CatchArrow (arrow);
        }
        base.Collider = collider;
        if ((this.input.JumpPressed || (bool)this.jumpBufferCounter) && (!this.DodgeSliding || this.CanUnduck ())) {
            if (this.CanDodgeWallJump (Facing.Left)) {
                if (!this.DodgeWallJump (1)) {
                    return 0;
                }
            } else if (this.CanDodgeWallJump (Facing.Right)) {
                if (!this.DodgeWallJump (-1)) {
                    return 0;
                }
            } else if ((bool)this.jumpGraceCounter) {
                if (this.DodgeSliding) {
                    this.UseNormalHitbox ();
                    this.DodgeSliding = false;
                    this.autoMove = Math.Sign (this.Speed.X);
                    this.scheduler.ScheduleAction (this.FinishAutoMove, 16, false);
                    if (Math.Abs (this.Speed.X) >= 1.75f) {
                        this.Speed.X = (float)Math.Sign (this.Speed.X) * 2.8f;
                    }
                    this.Jump (true, true, false, 0, false);
                    return 0;
                }
                this.Speed.X = MathHelper.Clamp (this.Speed.X, -1.8f, 1.8f);
                this.Jump (true, true, false, 0, false);
                return 0;
            }
        }
        if (this.DodgeSliding && !(bool)this.jumpGraceCounter && this.CanUnduck ()) {
            this.UseNormalHitbox ();
            this.DodgeSliding = false;
        }
        if (this.input.DodgePressed) {
            if (this.Prism != null) {
                this.Prism.CaptiveForceShatter ();
            }
            this.canHyper = true;
            if ((bool)this.dodgeCatchCounter && this.Speed.LengthSquared () >= 3.6f) {
                Sounds.char_miracleGrab.Play (210f, 1f);
            }
            return this.GetDodgeExitState ();
        }
        if ((bool)this.dodgeEndCounter) {
            this.dodgeEndCounter.Update ();
            return 3;
        }
        if ((bool)this.dodgeStallCounter && this.input.DodgeCheck) {
            this.dodgeStallCounter.Update ();
            return 3;
        }
        return this.GetDodgeExitState ();
    }

    public override int DuckingUpdate ()
    {
        
        if (this.onHotCoals && ((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce) {
            this.HotCoalsBounce ();
        }
        if (this.headSprite.CurrentFrame == this.ArcherData.SleepHeadFrame) {
            if (base.Scene.OnInterval (60)) {
                if (this.snoreIndex == 0) {
                    UnlockData.UnlockAchievement ("SLEEPY_MASTER");
                    this.ArcherData.SFX.Sleep.Play (base.X, 1f);
                }
                this.snoreIndex = (this.snoreIndex + 1) % 4;
                base.Level.ParticlesFG.Emit (Particles.YellowSleeping, base.Position + new Vector2 (3f, -3f));
            }
        } else if (this.snoreIndex > 0) {
            this.snoreIndex = 0;
            this.ArcherData.SFX.Sleep.Stop (true);
        }
        base.Level.Session.MatchStats [this.PlayerIndex].DuckFrames += Engine.TimeMult;
        this.wings.Normal ();
        bool flag = this.CanUnduck ();
        if (flag && !this.dodgeCooldown && this.input.DodgePressed && !base.Level.Session.MatchSettings.Variants.NoDodging [this.PlayerIndex]) {
            this.DodgeSliding = true;
            return 3;
        }
        if (this.input.JumpPressed && base.CollideCheck (GameTags.JumpThru, base.Position + Vector2.UnitY) && !base.CollideCheck (GameTags.Solid, base.Position + Vector2.UnitY * 3f)) {
            this.jumpBufferCounter.Set (0);
            this.jumpGraceCounter.Set (0);
            base.Y += 3f;
            Sounds.env_gplatPlayerDrop.Play (base.X, 1f);
            return 0;
        }
        if (flag) {
            if (this.onHotCoals && !((MyMatchVariants)Level.Session.MatchSettings.Variants).NoCoalBounce) {
                this.HotCoalsBounce ();
                return 0;
            }
            if (this.input.JumpPressed && (bool)this.jumpGraceCounter) {
                this.Jump (true, true, false, 0, false);
                return 0;
            }
            if (!this.OnGround || this.moveAxis.Y != 1f) {
                return 0;
            }
        }
        if (!Player.ShootLock) {
            if (this.input.AltShootPressed) {
                if (this.triggerArrows.Count > 0) {
                    this.DetonateTriggerArrows ();
                } else if (this.CanUnduck ()) {
                    this.Aiming = true;
                    this.startAimingDown = true;
                    return 0;
                }
            } else if (this.input.ShootPressed && this.CanUnduck ()) {
                this.Aiming = true;
                this.startAimingDown = true;
                return 0;
            }
        }
        float target = flag ? 0f : (0.8f * (float)this.input.MoveX);
        this.Speed.X = Calc.Approach (this.Speed.X, target, 0.4f * Engine.TimeMult);
        if (!this.OnGround) {
            this.Speed.Y = Calc.Approach (this.Speed.Y, 2.8f, 0.3f * Engine.TimeMult);
        }
        base.MoveH (this.Speed.X * Engine.TimeMult, this.onCollideH);
        base.MoveV (this.Speed.Y * Engine.TimeMult, this.onCollideV);
        if (Math.Sign (this.moveAxis.X) == 0 - this.Facing) {
            this.Facing = (Facing)(0 - this.Facing);
            this.bodySprite.Scale = new Vector2 (1.2f, 0.8f);
        }
        if (this.duckSlipCounter > 0f) {
            this.duckSlipCounter -= Engine.TimeMult;
        } else if (this.OnGround) {
            if (!base.CheckBelow (-3)) {
                base.MoveH (-1f * Engine.TimeMult, null);
                if (!base.CheckBelow ()) {
                    base.MoveV (1f, null);
                }
            } else if (!base.CheckBelow (3)) {
                base.MoveH (1f * Engine.TimeMult, null);
                if (!base.CheckBelow ()) {
                    base.MoveV (1f, null);
                }
            }
        }
        return 2;
    }
  }
}
