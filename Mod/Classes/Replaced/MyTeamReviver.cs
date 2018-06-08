using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using Mod;

namespace TowerFall
{
	public class MyTeamReviver : LevelEntity
	{
		//
		// Static Fields
		//
		public const int REVIVER_COOP_INVULN_TIME = 60;

		public const int REVIVE_DELAY = 60;

		public const int TEAM_DEATHMATCH_REVIVE_TIME = 60;

		public const int COOP_REVIVE_TIME = 60;

		public const int REVIVED_FINISH_DELAY = 5;

		public const int REVIVED_VOCAL_TIME = 15;

		public const int REVIVED_FROZEN_TIME = 15;

		public const int REVIVED_TEAM_DEATHMATCH_INVULN_TIME = 30;

		public const int REVIVED_COOP_INVULN_TIME = 120;

		public const int REVIVE_Y_OFFSET = -2;

		//
		// Fields
		//
		public bool levitateCorpse;

		public Color arrowColor;

		public Color colorA;

		public int reviver = -1;

		public SineWave arrowSine;

		public float targetLightAlpha;

		public bool canRevive;

		public Color colorB;

		public SineWave sine;

		public bool reviving;

		public float reviveCounter;

		public WrapHitbox playerNomralHitbox;

		public WrapHitbox revivingHitbox;

		public WrapHitbox normalHitbox;

		public TeamReviver.Modes Mode;

		public bool AutoRevive;

		public Vector2 targetPosition;

		public bool ghostRevives;

		//
		// Properties
		//
		public PlayerCorpse Corpse {
			get;
			set;
		}

		public bool Finished {
			get;
			set;
		}

		public bool PlayerCanRevive {
			get;
			set;
		}

		public int ReviveTime {
			get {
				return TEAM_DEATHMATCH_REVIVE_TIME;
				// switch (this.Mode) {
				// 	case TeamReviver.Modes.TeamDeathmatch: {
				// 		IL_18:
				// 		int result = 60;
				// 		return result;
				// 	}
				// 	case TeamReviver.Modes.DarkWorld: {
				// 		int result = 60;
				// 		return result;
				// 	}
				// }
				// goto IL_18;
			}
		}

		//
		// Constructors
		//
		public MyTeamReviver (PlayerCorpse corpse, TeamReviver.Modes mode, bool ghostRevives=false) : base (corpse.BottomCenter)
		{
			this.ghostRevives = ghostRevives;
			this.Mode = mode;
			this.Corpse = corpse;
			this.ScreenWrap = true;
			base.Tag (new GameTags[] {
				GameTags.LightSource,
				GameTags.Dummy // Using this tag because it doesn't appear in 8-player
			});
			this.LightRadius = 60f;
			this.LightAlpha = 1f;
			base.Collider = (this.normalHitbox = new WrapHitbox (24f, 25f, -12f, -20f));
			this.revivingHitbox = new WrapHitbox (40f, 46f, -20f, -30f);
			this.playerNomralHitbox = new WrapHitbox (8f, 14f, -4f, -6f);
			this.reviveCounter = (float)this.ReviveTime;
			base.Add (this.sine = new SineWave (90));
			base.Add (this.arrowSine = new SineWave (20));
			switch (this.Mode) {
			case TeamReviver.Modes.TeamDeathmatch:
				this.arrowColor = (this.colorA = ArcherData.Get (corpse.TeamColor).ColorA);
				this.colorB = ArcherData.Get (corpse.TeamColor).ColorB;
				this.PlayerCanRevive = true;
				break;
			case TeamReviver.Modes.DarkWorld: {
				ArcherData archerData = ArcherData.Get (TFGame.Characters [corpse.PlayerIndex], TFGame.AltSelect [corpse.PlayerIndex]);
				this.arrowColor = (this.colorA = archerData.ColorA);
				this.colorB = archerData.ColorB;
				this.PlayerCanRevive = true;
				break;
			}
			case TeamReviver.Modes.Quest: {
				ArcherData archerData = ArcherData.Get (TFGame.Characters [corpse.PlayerIndex], TFGame.AltSelect [corpse.PlayerIndex]);
				this.arrowColor = (this.colorA = archerData.ColorA);
				this.colorB = archerData.ColorB;
				this.PlayerCanRevive = false;
				break;
			}
			}
			Alarm.Set (this, 60, delegate {
				this.canRevive = true;
			}, Alarm.AlarmMode.Oneshot);
			this.targetLightAlpha = 1f;
		}

		//
		// Methods
		//
		public bool CanReviveAtThisPosition (ref Vector2 revivePoint)
		{
			Collider collider = base.Collider;
			Vector2 position = this.Position;
			base.Collider = this.playerNomralHitbox;
			base.BottomCenter = this.Corpse.BottomCenter;
			Vector2 position2 = this.Position;
			bool result;
			for (int i = 0; i < 10; i += 2) {
				this.Position = position2 + Vector2.UnitY * (float)i;
				if (!base.CollideCheck (GameTags.Solid)) {
					revivePoint = this.Position;
					base.Collider = collider;
					this.Position = position;
					result = true;
					return result;
				}
			}
			base.Collider = collider;
			this.Position = position;
			result = false;
			return result;
		}

		public bool CanReviveAtThisPosition ()
		{
			Vector2 zero = Vector2.Zero;
			return this.CanReviveAtThisPosition (ref zero);
		}

		public override void DrawLight (LightingLayer layer)
		{
			Vector2 position = this.Position;
			this.Position += Vector2.UnitY * -4f;
			base.DrawLight (layer);
			this.Position = position;
		}

		public Player FinishReviving ()
		{
			Vector2 zero = Vector2.Zero;
			Player result;
			if (this.Corpse.Squished == Vector2.Zero && this.CanReviveAtThisPosition (ref zero)) {
				PlayerInventory inventory = new PlayerInventory (false, false, false, false, new ArrowList (this.Corpse.Arrows));
				// this.Corpse.Arrows.Clear (); // I don't know what this line does, but it was causing an accessibility exception
				if (this.Corpse.ArrowCushion.Count > 0) {
					Arrow arrow = this.Corpse.ArrowCushion.ArrowDatas [0].Arrow;
					if (inventory.Arrows.CanAddArrow (arrow.ArrowType) && arrow.Scene != null && !arrow.MarkedForRemoval) {
						base.Level.Remove<Arrow> (arrow);
						inventory.Arrows.AddArrows (new ArrowTypes[] {
							arrow.ArrowType
						});
					}
					this.Corpse.ArrowCushion.ReleaseArrows (Vector2.UnitY * -1f);
				}
				if (this.Mode == TeamReviver.Modes.Quest && inventory.Arrows.Count == 0) {
					ArrowList arg_153_0 = inventory.Arrows;
					ArrowTypes[] arrows = new ArrowTypes[1];
					arg_153_0.AddArrows (arrows);
				}
				Vector2 position = zero;
				Player player = new Player (this.Corpse.PlayerIndex, position, this.Corpse.Allegiance, this.Corpse.TeamColor, inventory, Player.HatStates.NoHat, true, false, false);
				if (this.Mode == TeamReviver.Modes.TeamDeathmatch) {
					player.Flash (45, null);
				} else {
					player.Flash (135, null);
				}
				base.Level.Add<Player> (player);
				int playerIndex = this.reviver;
				if (playerIndex == -1) {
					playerIndex = player.PlayerIndex;
				}
				if (this.Mode == TeamReviver.Modes.DarkWorld) {
					Player player2 = base.Level.GetPlayer (this.reviver);
					if (player2 != null) {
						player2.Flash (60, null);
					}
				}
				ShockCircle shockCircle = Cache.Create<ShockCircle> ();
				shockCircle.Init (position, playerIndex, player, ShockCircle.ShockTypes.TeamRevive);
				base.Level.Add<ShockCircle> (shockCircle);
				int num = Calc.Random.Next (360);
				for (int i = num; i < num + 360; i += 30) {
					base.Level.Add<BombParticle> (new BombParticle (position, (float)i, BombParticle.Type.TeamRevive));
				}
				TFGame.PlayerInputs [this.Corpse.PlayerIndex].Rumble (1f, 30);
				if (this.reviver != -1) {
					TFGame.PlayerInputs [this.reviver].Rumble (0.5f, 30);
				}
				if (this.reviver != -1) {
					MatchStats[] expr_2FA_cp_0 = base.Level.Session.MatchStats;
					int expr_2FA_cp_1 = this.reviver;
					expr_2FA_cp_0 [expr_2FA_cp_1].Revives = expr_2FA_cp_0 [expr_2FA_cp_1].Revives + 1u;
					if (base.Level.Session.DarkWorldState != null) {
						base.Level.Session.DarkWorldState.Revives [this.reviver]++;
					}
				}
				this.reviveCounter = 0f;
				if (this.Corpse.TeamColor == Allegiance.Red) {
					Sounds.sfx_reviveRedteamFinish.Play (210f, 1f);
				} else {
					Sounds.sfx_reviveBlueteamFinish.Play (210f, 1f);
				}
				result = player;
			} else {
				result = null;
			}

			// If ghost revives is on, then a revive can cancel a level ending
			if (this.ghostRevives && base.Level.Session.MatchSettings.Mode == Modes.TeamDeathmatch) {
				Allegiance allegiance;
				if (!base.Level.Session.RoundLogic.TeamCheckForRoundOver(out allegiance)) {
					base.Level.Session.CurrentLevel.Ending = false;
				}
			}

			return result;
		}

		public void HUDRender ()
		{
			if (!this.Finished && /*!base.Level.Ending &&*/ !this.Corpse.PrismHit && this.Mode != TeamReviver.Modes.Quest) {
				float num = MathHelper.Lerp (-1f, this.arrowSine.Value, this.reviveCounter / (float)this.ReviveTime) * 2f;
				Draw.OutlineTextureCentered (TFGame.Atlas ["versus/playerIndicator"], this.Position + new Vector2 (0f, -18f + num), this.arrowColor);
				Draw.OutlineTextureCentered (TFGame.Atlas ["versus/teamRevive"], this.Position + new Vector2 (0f, -28f + num), this.arrowColor);
			}
		}

		public void ResetCounter ()
		{
			this.reviveCounter = (float)this.ReviveTime;
		}

		public IEnumerator ReviveSequence ()
		{
			using (List<Entity>.Enumerator enumerator = base.Level [GameTags.PlayerGhost].GetEnumerator ()) {
				while (enumerator.MoveNext ()) {
					PlayerGhost playerGhost = (PlayerGhost)enumerator.Current;
					if (playerGhost.PlayerIndex == this.Corpse.PlayerIndex) {
						playerGhost.Despawn (this.Corpse);
						break;
					}
				}
			}
			yield return 5;
			Player player = this.FinishReviving ();
			if (player == null) {
				this.levitateCorpse = false;
				this.Finished = false;
				this.reviving = false;
				this.Corpse.Reviving = false;
				this.Corpse.Revived = false;
				if (base.Level.Session.MatchSettings.Mode == TowerFall.Modes.DarkWorld) {
					(base.Level.Session.RoundLogic as DarkWorldRoundLogic).CheckForGameOver ();
				} else if (base.Level.Session.MatchSettings.Mode == TowerFall.Modes.TeamDeathmatch) {
					(base.Level.Session.RoundLogic as TeamDeathmatchRoundLogic).CheckForWin ();
				}
			} else {
				yield return 1;
				base.Level.Remove<PlayerCorpse> (this.Corpse);
				this.targetLightAlpha = 0f;
				yield return 15;
				player.ArcherData.SFX.Revive.Play (base.X, 1f);
				yield return 15;
				if (player != null) {
					player.Unfreeze ();
				}
				base.RemoveSelf ();
			}
			yield break;
		}

		public void ReviveUpdate ()
		{
			this.LightAlpha = Calc.Approach (this.LightAlpha, this.targetLightAlpha, 0.1f * Engine.TimeMult);
			base.Update ();
			if (this.levitateCorpse) {
				float num = this.targetPosition.Y + this.sine.Value * 2f;
				Vector2 zero = Vector2.Zero;
				zero.Y = MathHelper.Clamp (num - this.Corpse.ActualPosition.Y, -0.6f, 0.6f);
				if (!this.Finished && !this.AutoRevive) {
					Player player = base.Level.GetPlayer (this.reviver);
					if (player != null) {
						if (Math.Abs (player.X - base.X) > 10f) {
							zero.X = (float)Math.Sign (player.X - base.X) * 0.2f;
						}
						if (Math.Abs (player.Y - base.Y) > 14f) {
							zero.Y = (float)Math.Sign (player.Y - base.Y) * 0.6f;
						}
					}
				}
				if (this.Corpse.Squished == Vector2.Zero) {
					this.Corpse.Speed = this.Corpse.Speed.Approach (zero, 4f * Engine.TimeMult);
				}
			}
			if (!this.Finished) {
				if (this.Corpse.Scene == null || this.Corpse.MarkedForRemoval) {
					LightFade lightFade = Cache.Create<LightFade> ();
					lightFade.Init (this, null);
					base.Level.Add<LightFade> (lightFade);
					base.RemoveSelf ();
				} else {
					this.Position = this.Corpse.BottomCenter;
					if (base.Scene.OnInterval (3)) {
						if (this.arrowColor == this.colorA) {
							this.arrowColor = this.colorB;
						} else {
							this.arrowColor = this.colorA;
						}
					}
					if (this.reviving) {
						this.reviveCounter -= Engine.TimeMult;
						if (this.Corpse.Squished != Vector2.Zero) {
							this.StopReviving ();
						} else if (this.reviveCounter <= 0f) {
							this.Finished = true;
							this.Corpse.Revived = true;
							base.Add (new Coroutine (this.ReviveSequence ()));
						} else if (!this.Corpse.PrismHit && this.CanReviveAtThisPosition ()) {
							if (!this.AutoRevive && this.PlayerCanRevive) {
								bool flag = false;
								int num2 = -1;
								using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
									while (enumerator.MoveNext ()) {
										Player player2 = (Player)enumerator.Current;
										if (player2.Allegiance == this.Corpse.Allegiance && base.CollideCheck (player2)) {
											flag = true;
											if (num2 != this.reviver) {
												if (player2.PlayerIndex == this.reviver) {
													num2 = this.reviver;
												} else if (num2 == -1) {
													num2 = player2.PlayerIndex;
												}
											}
										}
									}
								}

								if (this.ghostRevives) {
									using (List<Entity>.Enumerator enumerator = base.Level[GameTags.PlayerGhost].GetEnumerator ()) {
										while (enumerator.MoveNext ()) {
											PlayerGhost ghost = (PlayerGhost)enumerator.Current;
											if (ghost.Allegiance == this.Corpse.Allegiance && base.CollideCheck (ghost) && ghost.PlayerIndex != this.Corpse.PlayerIndex) {
												flag = true;
												if (num2 != this.reviver) {
													if (ghost.PlayerIndex == this.reviver) {
														num2 = this.reviver;
													} else if (num2 == -1) {
														num2 = ghost.PlayerIndex;
													}
												}
											}
										}
									}
								}

								if (num2 != this.reviver && num2 != -1) {
									this.reviver = num2;
								}
								if (!flag) {
									this.StopReviving ();
								}
							}
							TFGame.PlayerInputs [this.Corpse.PlayerIndex].Rumble (0.5f, 2);
							if (this.reviver != -1) {
								TFGame.PlayerInputs [this.reviver].Rumble (0.5f, 2);
							}
						} else {
							this.StopReviving ();
						}
					} else {
						this.ResetCounter ();
						this.LightAlpha = Calc.Approach (this.LightAlpha, 0f, 0.1f * Engine.TimeMult);
						if (this.canRevive && !this.Corpse.PrismHit && this.Corpse.Squished == Vector2.Zero && this.CanReviveAtThisPosition ()) {
							if (this.AutoRevive) {
								this.StartReviving ();
							} else if (this.PlayerCanRevive) {
								using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
									while (enumerator.MoveNext ()) {
										Player player2 = (Player)enumerator.Current;
										if (player2.Allegiance == this.Corpse.Allegiance && !player2.Dead && base.CollideCheck (player2)) {
											this.reviver = player2.PlayerIndex;
											this.StartReviving ();
											break;
										}
									}
								}

								if (this.ghostRevives) {
									using (List<Entity>.Enumerator enumerator = base.Level[GameTags.PlayerGhost].GetEnumerator ()) {
										while (enumerator.MoveNext ()) {
											PlayerGhost ghost = (PlayerGhost)enumerator.Current;
											if (ghost.Allegiance == this.Corpse.Allegiance && ghost.State != 3 && base.CollideCheck (ghost) && ghost.PlayerIndex != this.Corpse.PlayerIndex) {
												this.reviver = ghost.PlayerIndex;
												this.StartReviving ();
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void StartReviving ()
		{
			this.reviving = true;
			base.Collider = this.revivingHitbox;
			this.targetPosition = this.Corpse.Position + Vector2.UnitY * -6f;
			this.sine.Restart ();
			this.Corpse.Reviving = true;
			this.Corpse.Pinned = false;
			this.Corpse.Ledge = 0;
			this.Corpse.IgnoreJumpThrus = true;
			this.levitateCorpse = true;
			if (this.Corpse.TeamColor == Allegiance.Red) {
				Sounds.sfx_reviveRedteamStart.Play (base.X, 1f);
			} else {
				Sounds.sfx_reviveBlueteamStart.Play (base.X, 1f);
			}
		}

		public void StopReviving ()
		{
			this.reviving = false;
			this.reviver = -1;
			base.Collider = this.normalHitbox;
			this.Corpse.Reviving = false;
			this.Corpse.IgnoreJumpThrus = false;
			this.levitateCorpse = false;
			if (this.Corpse.TeamColor == Allegiance.Red) {
				Sounds.sfx_reviveRedteamStart.Stop (true);
				Sounds.sfx_reviveRedteamCancel.Play (base.X, 1f);
			} else {
				Sounds.sfx_reviveBlueteamStart.Stop (true);
				Sounds.sfx_reviveBlueteamCancel.Play (base.X, 1f);
			}
		}

		public override void Update ()
		{
		}

		// //
		// // Nested Types
		// //
		// public enum Modes
		// {
		// 	TeamDeathmatch,
		// 	DarkWorld,
		// 	Quest
		// }
	}
}
