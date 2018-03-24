using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace TowerFall
{
	public class MyChalice : LevelEntity
	{
		//
		// Fields
		//
		public Image image;

		public MyChalicePad pad;

		public bool finished;

		public bool flash;

		//
		// Constructors
		//
		public MyChalice (MyChalicePad pad)
		{
			this.pad = pad;
			pad.Chalice = this;
			this.Position = pad.TopCenter;
			this.image = new Image (TFGame.Atlas ["chalice"], null);
			this.image.JustifyOrigin (0.5f, 1f);
			base.Add (this.image);
			base.Tag (GameTags.LightSource);
			this.LightRadius = 60f;
			this.LightAlpha = 0f;
		}

		//
		// Methods
		//
		public override void DrawLight (LightingLayer layer)
		{
			Vector2 position = this.Position;
			this.Position += new Vector2 (0f, -34f);
			base.DrawLight (layer);
			this.Position = position;
		}

		public void Finish ()
		{
			this.finished = true;
			base.Level.Add<ScreenFlash> (new ScreenFlash (Color.White, 0.5f));
			base.Level.Add<MyChaliceGhost> (new MyChaliceGhost (this.pad.OwnerIndex, this));
		}

		public override void Render ()
		{
			if (base.Scene.OnInterval (5)) {
				this.flash = !this.flash;
			}
			if (this.pad.OwnerIndex != -1) {
				Subtexture subtexture = TFGame.Atlas ["chaliceBlood"];
				int num = (int)MathHelper.Lerp (0f, (float)subtexture.Height, this.pad.FillPercent);
				if (num > 0) {
					Color color;
					if (base.Level.Session.MatchSettings.TeamMode) {
						TeamData teamData = ArcherData.Get (base.Level.Session.MatchSettings.Teams [this.pad.OwnerIndex]);
						color = (this.flash ? teamData.ColorB : teamData.ColorA);
					} else {
						ArcherData archerData = ArcherData.Get (TFGame.Characters [this.pad.OwnerIndex], ArcherData.ArcherTypes.Normal);
						color = (this.flash ? archerData.ColorB : archerData.ColorA);
					}
					Draw.Texture (subtexture, new Rectangle (0, subtexture.Height - num, subtexture.Width, num), this.Position + new Vector2 (-8f, (float)(-40 + subtexture.Height - num)), color);
				}
			}
			this.image.DrawOutline (1);
			base.Render ();
		}

		public override void Update ()
		{
			base.Update ();
			this.LightAlpha = this.pad.FillPercent;
		}
	}
}
