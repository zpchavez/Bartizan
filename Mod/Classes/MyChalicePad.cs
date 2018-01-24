using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Xml;

namespace TowerFall
{
	public class MyChalicePad : Solid
	{
		//
		// Static Fields
		//
		public const float FILL_RATE = 0.00740740728f;

		public const float DRAIN_START = 0.00148148148f;

		public const float DRAIN_END = 0.0148148146f;

		public const int DRAIN_LERP_TIME = 60;

		//
		// Fields
		//
		public List<int> contestors;

		public float drainLerp;

		public bool flash;

		public Color colorB;

		public int sfxState = -1;

		public Image[] images;

		public bool finished;

		public MyChalice Chalice;

		public Color colorA;

		//
		// Properties
		//
		public float FillPercent {
			get;
			set;
		}

		public int OwnerIndex {
			get;
			set;
		}

		public Allegiance OwnerTeam {
			get;
			set;
		}

		//
		// Constructors
		//
		public MyChalicePad (XmlElement xml) : this (xml.Position (), xml.AttrInt ("width"))
		{
		}

		public MyChalicePad (Vector2 position, int width) : base (position, width, 10, false)
		{
			// Don't need to do any constructing. Only need this to pass into Chalice constructor.
		}

		//
		// Methods
		//
		public override void DrawLight (LightingLayer layer)
		{
			if (this.LightAlpha > 0f) {
				float x = base.X;
				base.X += 5f;
				int num = 0;
				while ((float)num < base.Width) {
					base.DrawLight (layer);
					base.X += 10f;
					num += 10;
				}
				base.X = x;
			}
		}

		public void Finish ()
		{
			this.finished = true;
			this.Chalice.Finish ();
		}

		public bool GetContestorOnPad (ref int id)
		{
			bool result = false;
			this.contestors.Clear ();
			if (base.Level.Session.MatchSettings.TeamMode) {
				Allegiance allegiance = Allegiance.Neutral;
				if (this.OwnerIndex != -1) {
					allegiance = base.Level.Session.MatchSettings.Teams [this.OwnerIndex];
				}
				if (allegiance != Allegiance.Blue) {
					using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
						while (enumerator.MoveNext ()) {
							Player player = (Player)enumerator.Current;
							if (!player.Dead && player.Allegiance == Allegiance.Blue && base.CollideCheck (player, this.Position - Vector2.UnitY)) {
								this.contestors.Add (player.PlayerIndex);
								result = true;
								break;
							}
						}
					}
				}
				if (allegiance != Allegiance.Red) {
					using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
						while (enumerator.MoveNext ()) {
							Player player = (Player)enumerator.Current;
							if (!player.Dead && player.Allegiance == Allegiance.Red && base.CollideCheck (player, this.Position - Vector2.UnitY)) {
								this.contestors.Add (player.PlayerIndex);
								result = true;
								break;
							}
						}
					}
				}
			} else {
				using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
					while (enumerator.MoveNext ()) {
						Player player = (Player)enumerator.Current;
						if (!player.Dead && player.PlayerIndex != this.OwnerIndex && base.CollideCheck (player, this.Position - Vector2.UnitY)) {
							this.contestors.Add (player.PlayerIndex);
							result = true;
						}
					}
				}
			}
			if (this.contestors.Count == 1) {
				id = this.contestors [0];
			} else {
				id = -1;
			}
			return result;
		}

		public int GetFriendlyOnPad ()
		{
			int result;
			if (this.OwnerIndex == -1 || !base.Level.Session.MatchSettings.TeamMode) {
				result = -1;
			} else {
				Allegiance allegiance = base.Level.Session.MatchSettings.Teams [this.OwnerIndex];
				using (List<Entity>.Enumerator enumerator = base.Level.Players.GetEnumerator ()) {
					while (enumerator.MoveNext ()) {
						Player player = (Player)enumerator.Current;
						if (!player.Dead && player.Allegiance == allegiance && base.CollideCheck (player, this.Position - Vector2.UnitY)) {
							result = player.PlayerIndex;
							return result;
						}
					}
				}
				result = -1;
			}
			return result;
		}

		public bool IsContestorOnPad ()
		{
			int num = 0;
			return this.GetContestorOnPad (ref num);
		}

		public bool IsOwnerOnPad ()
		{
			bool result;
			if (this.OwnerIndex == -1) {
				result = false;
			} else {
				Player player = base.Level.GetPlayer (this.OwnerIndex);
				result = (player != null && !player.Dead && base.CollideCheck (player, this.Position - Vector2.UnitY));
			}
			return result;
		}

		public void SetSFXState (int set)
		{
			if (set != this.sfxState) {
				this.sfxState = set;
				if (set == 1) {
					Sounds.sfx_chaliceFill.Play (base.X, 1f);
				} else if (set == -1) {
					Sounds.sfx_chaliceCancel.Play (base.X, 1f);
				}
			}
		}

		public override void Update ()
		{
			base.Update ();
			if (base.Scene.OnInterval (5)) {
				this.flash = !this.flash;
			}
			int num = -1;
			bool flag = false;
			if (this.finished) {
				num = this.OwnerIndex;
			} else if (this.IsOwnerOnPad ()) {
				num = this.OwnerIndex;
				flag = this.IsContestorOnPad ();
			} else {
				int friendlyOnPad = this.GetFriendlyOnPad ();
				if (friendlyOnPad != -1) {
					num = friendlyOnPad;
					flag = this.IsContestorOnPad ();
				} else {
					this.GetContestorOnPad (ref num);
				}
			}
			Allegiance allegiance;
			if (num == -1) {
				allegiance = Allegiance.Neutral;
			} else {
				allegiance = base.Level.Session.MatchSettings.Teams [num];
			}
			float num3;
			if (base.Level.Session.MatchSettings.TeamMode) {
				int num2 = base.Level.Session.MatchSettings.GetTeamMismatch ((Allegiance)this.OwnerIndex);
				num2 = Math.Sign (num2);
				num3 = 1f / (float)(120 + 15 * num2);
			} else {
				num3 = 1f / (float)(120 - 15 * (TFGame.PlayerAmount - 2));
			}
			float num4 = MathHelper.Lerp (0.00148148148f, 0.0148148146f, this.drainLerp);
			if (flag) {
				this.SetSFXState (-1);
			} else if (num == -1) {
				this.SetSFXState (-1);
				this.FillPercent = Calc.Approach (this.FillPercent, 0f, num4 * Engine.TimeMult);
				this.drainLerp = Calc.Approach (this.drainLerp, 1f, 0.0166666675f * Engine.TimeMult);
				if (this.FillPercent == 0f) {
					this.OwnerIndex = -1;
					this.OwnerTeam = Allegiance.Neutral;
				}
			} else {
				if (num != this.OwnerIndex && allegiance == this.OwnerTeam && base.Level.Session.MatchSettings.TeamMode) {
					this.OwnerIndex = num;
				}
				if (num == this.OwnerIndex) {
					this.SetSFXState (1);
					this.FillPercent = Calc.Approach (this.FillPercent, 1f, num3 * Engine.TimeMult);
					this.drainLerp = 0f;
					if (this.FillPercent >= 1f && !this.finished) {
						this.Finish ();
					}
				} else {
					this.SetSFXState (-1);
					this.FillPercent = Calc.Approach (this.FillPercent, 0f, num4 * Engine.TimeMult);
					this.drainLerp = Calc.Approach (this.drainLerp, 1f, 0.0166666675f * Engine.TimeMult);
					if (this.FillPercent == 0f) {
						this.OwnerIndex = num;
						this.OwnerTeam = allegiance;
					}
				}
			}
			if (num == -1) {
				this.LightAlpha = Math.Max (this.LightAlpha - 0.1f * Engine.TimeMult, 0f);
			} else {
				this.LightAlpha = Math.Min (this.LightAlpha + 0.1f * Engine.TimeMult, 1f);
			}
			int num5 = num;
			if (flag) {
				num5 = this.OwnerIndex;
			}
			Allegiance team = allegiance;
			if (flag) {
				team = this.OwnerTeam;
			}
			if (num5 == -1) {
				this.LightColor = Color.Black;
				Image[] array = this.images;
				for (int i = 0; i < array.Length; i++) {
					Image image = array [i];
					image.Color = Color.Gray;
				}
			} else {
				if (base.Level.Session.MatchSettings.TeamMode) {
					this.colorA = ArcherData.Get (team).ColorA;
					this.colorB = ArcherData.Get (team).ColorB;
				} else {
					this.colorA = ArcherData.Get (TFGame.Characters [num5], ArcherData.ArcherTypes.Normal).ColorA;
					this.colorB = ArcherData.Get (TFGame.Characters [num5], ArcherData.ArcherTypes.Normal).ColorB;
				}
				Image[] array = this.images;
				for (int i = 0; i < array.Length; i++) {
					Image image = array [i];
					image.Color = (this.flash ? this.colorA : this.colorB);
				}
				this.LightColor = this.colorB.Invert ();
			}
		}
	}
}
