using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using Mod;

namespace TowerFall
{
	public class MyChaliceGhost : LevelEntity
	{
		//
		// Static Fields
		//
		public const float ALPHA = 0.75f;

		//
		// Fields
		//
		public Wiggler wiggler;

		public Sprite<string> cloak;

		public Color colorB;

		public Color colorA;

		public bool flash;

		public bool spawned;

		public bool targetIsGhost;

		public PlayerGhost ghostTarget;

		public Player playerTarget;

		public bool dead;

		public Sprite<string> sprite;

		public float lerp;

		public Vector2 speed;

		public Actor target;

		public MyChalice source;

		public bool canFindTarget;

		public Allegiance team;

		public int ownerIndex;

		public bool huntsGhosts;

		//
		// Constructors
		//
		public MyChaliceGhost (int ownerIndex, MyChalice source, bool huntsGhosts=false)
		{
			base.Depth = -1000000;
			base.Tag (new GameTags[] {
				GameTags.PlayerCollider,
				GameTags.PlayerGhostCollider,
				GameTags.LightSource
			});
			base.Collider = new WrapHitbox (14f, 14f, -7f, -7f);
			this.Collidable = false;
			this.huntsGhosts = huntsGhosts;
			this.ownerIndex = ownerIndex;
			this.source = source;
			this.ScreenWrap = true;
			this.LightAlpha = 0f;
			this.LightRadius = 60f;
			this.cloak = TFGame.SpriteData.GetSpriteString ("ChaliceGhostTeamColor");
			base.Add (this.cloak);
			this.sprite = TFGame.SpriteData.GetSpriteString ("ChaliceGhost");
			this.sprite.Play ("spawn", false);
			this.sprite.Color = Color.White * 0.75f;
			base.Add (this.sprite);
			this.sprite.OnAnimationComplete = delegate (Sprite<string> s) {
				if (s.CurrentAnimID == "attack") {
					s.Play ("idle", false);
				} else if (s.CurrentAnimID == "spawn") {
					s.Play ("idle", false);
					this.spawned = true;
				} else if (s.CurrentAnimID == "death") {
					base.RemoveSelf ();
				}
			};
			this.Position = source.Position + new Vector2 (0f, -34f);
			Tween tween = Tween.Create (Tween.TweenMode.Oneshot, Ease.BackOut, 15, true);
			tween.OnUpdate = delegate (Tween t) {
				this.LightAlpha = t.Eased;
			};
			base.Add (tween);
			base.Add (new Coroutine (this.Sequence ()));
			this.wiggler = Wiggler.Create (30, 4f, null, delegate (float v) {
				this.sprite.Scale = Vector2.One * (1f + v * 0.2f);
			}, false, false);
			base.Add (this.wiggler);
			Sounds.sfx_chaliceGhostAppear.Play (210f, 1f);
		}

		//
		// Methods
		//
		public override void Added ()
		{
			base.Added ();
			if (base.Level.Session.MatchSettings.TeamMode) {
				this.team = base.Level.Session.MatchSettings.Teams [this.ownerIndex];
				this.colorA = ArcherData.Get (this.team).ColorA;
				this.colorB = ArcherData.Get (this.team).ColorB;
			} else {
				this.team = Allegiance.Neutral;
				this.colorA = ArcherData.Get (TFGame.Characters [this.ownerIndex], ArcherData.ArcherTypes.Normal).ColorA;
				this.colorB = ArcherData.Get (TFGame.Characters [this.ownerIndex], ArcherData.ArcherTypes.Normal).ColorB;
			}
			this.cloak.Color = this.colorB * 0.75f;
		}

		public bool CanAttack (Player player)
		{
			bool result;
			if (this.team != Allegiance.Neutral) {
				result = (player.Allegiance != this.team);
			} else {
				result = (player.PlayerIndex != this.ownerIndex);
			}
			return result;
		}

		public bool CanAttackGhost (PlayerGhost ghost)
		{
			bool result;
			if (this.team != Allegiance.Neutral) {
				result = (ghost.Allegiance != this.team);
			} else {
				result = (ghost.PlayerIndex != this.ownerIndex);
			}
			return result;
		}

		public Actor getGhostTarget(float maxDistSq)
		{
			PlayerGhost result = null;
			float num = maxDistSq;
			using (List<Entity>.Enumerator enumerator = base.Level [GameTags.PlayerGhost].GetEnumerator ()) {
				while (enumerator.MoveNext ()) {
					PlayerGhost ghost = (PlayerGhost)enumerator.Current;
					if (this.CanAttackGhost (ghost)) {
						float num2 = WrapMath.WrapDistanceSquared (this.Position, ghost.Position);
						if (num2 < num) {
							num = num2;
							result = ghost;
						}
					}
				}
			}
			if (result != null) {
				targetIsGhost = true;
				ghostTarget = result;
			}
			return result;
		}

		public Actor getPlayerTarget(float maxDistSq)
		{
			Player result = null;
			float num = maxDistSq;
			using (List<Entity>.Enumerator enumerator = base.Level [GameTags.Player].GetEnumerator ()) {
				while (enumerator.MoveNext ()) {
					Player player = (Player)enumerator.Current;
					if (this.CanAttack (player)) {
						float num2 = WrapMath.WrapDistanceSquared (this.Position, player.Position);
						if (num2 < num) {
							num = num2;
							result = player;
						}
					}
				}
			}
			if (result != null) {
				targetIsGhost = false;
				playerTarget = result;
			}
			return result;
		}

		public Actor GetClosestTarget (float maxDistSq)
		{
			Actor result = null;
			float num = maxDistSq;
			Random rand = new Random();

			if (this.huntsGhosts) {
				// flip a coin to determine whether to check for ghosts first or players first
				if (rand.Next(2) == 0) {
					result = getPlayerTarget(maxDistSq);
					if (result == null) {
						result = getGhostTarget(maxDistSq);
					}
				} else {
					result = getGhostTarget(maxDistSq);
					if (result == null) {
						result = getPlayerTarget(maxDistSq);
					}
				}
			} else {
				return getPlayerTarget(maxDistSq);
			}

			return result;
		}

		public Actor GetClosestTarget ()
		{
			return this.GetClosestTarget (3.40282347E+38f);
		}

		public Vector2 GetTargetSpeed ()
		{
			return WrapMath.Shortest (this.Position, this.target.Position).SafeNormalize (MathHelper.Lerp (1.2f, 2.4f, this.lerp));
		}

		public override void OnPlayerCollide (Player player)
		{
			if (player.CanHurt && this.CanAttack (player)) {
				this.sprite.Play ("attack", true);
				this.speed = (player.Position - this.Position).SafeNormalize (2f);
				player.Hurt (DeathCause.Chalice, this.Position, this.ownerIndex, null);
				this.canFindTarget = false;
				this.target = null;
				this.playerTarget = null;
				Alarm.Set (this, 30, delegate {
					this.canFindTarget = true;
				}, Alarm.AlarmMode.Oneshot);
				Sounds.sfx_chaliceGhostKill.Play (210f, 1f);
				this.wiggler.Start ();
			}
		}

		public override void OnPlayerGhostCollide (PlayerGhost ghost)
		{
			if (this.CanAttackGhost (ghost)) {
				this.sprite.Play ("attack", true);
				this.speed = (ghost.Position - this.Position).SafeNormalize (2f);
				ghost.Die (this.ownerIndex);
				this.canFindTarget = false;
				this.target = null;
				this.ghostTarget = null;
				this.targetIsGhost = false;
				Alarm.Set (this, 30, delegate {
					this.canFindTarget = true;
				}, Alarm.AlarmMode.Oneshot);
				Sounds.sfx_chaliceGhostKill.Play (210f, 1f);
				this.wiggler.Start ();
			}
		}

		public override void Removed ()
		{
			base.Scene.Add<LightFade> (Cache.Create<LightFade> ().Init (this, null));
			base.Removed ();
		}

		public IEnumerator Sequence ()
		{
			while (!this.spawned) {
				yield return null;
			}
			this.wiggler.Start ();
			yield return 20;
			this.Collidable = true;
			this.canFindTarget = true;
			yield return 90;
			Tween tween = Tween.Create (Tween.TweenMode.Oneshot, null, 480, true);
			tween.OnUpdate = delegate (Tween t) {
				this.lerp = t.Eased;
			};
			base.Add (tween);
			yield break;
		}

		public override void Update ()
		{
			base.Update ();
			if (base.Scene.OnInterval (5)) {
				this.flash = !this.flash;
			}
			if (this.flash) {
				this.cloak.Color = this.colorA * 0.75f;
			} else {
				this.cloak.Color = this.colorB * 0.75f;
			}
			if (this.speed.X != 0f) {
				this.sprite.FlipX = (this.speed.X < 0f);
			}
			this.cloak.Scale = this.sprite.Scale;
			this.cloak.CurrentFrame = this.sprite.CurrentFrame;
			this.cloak.FlipX = this.sprite.FlipX;
			if (this.target != null) {
				if (this.targetIsGhost && this.ghostTarget.State == 3) {
					this.target = null;
				} else if (this.playerTarget != null && this.playerTarget.Dead) {
					this.target = null;
				}
			}
			if (this.canFindTarget) {
				if (this.target == null) {
					this.target = this.GetClosestTarget ();
				} else {
					float num = (this.target.Position - this.Position).LengthSquared ();
					num -= 400f;
					Actor closestTarget = this.GetClosestTarget (num);
					if (closestTarget != null) {
						this.target = closestTarget;
					}
				}
			}
			if (this.target != null) {
				this.speed = this.speed.Approach (this.GetTargetSpeed (), MathHelper.Lerp (0.06f, 0.15f, this.lerp) * Engine.TimeMult);
			} else {
				this.speed = this.speed.Approach (Vector2.Zero, 0.1f * Engine.TimeMult);
			}
			this.Position += this.speed * Engine.TimeMult;
			if (!this.dead && this.sprite.CurrentAnimID == "idle") {
				bool flag = this.GetClosestTarget() != null;
				if (!flag) {
					this.Vanish ();
				}
			} else if (this.dead && this.sprite.CurrentAnimID == "idle") {
				this.Vanish ();
			}
		}

		public void Vanish ()
		{
			Sounds.sfx_chaliceGhostDisappear.Play (base.X, 1f);
			this.dead = true;
			this.sprite.Play ("death", false);
		}
	}
}
