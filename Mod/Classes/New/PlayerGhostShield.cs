using System;
using Monocle;
using TowerFall;

namespace New
{
	public class PlayerGhostShield : PlayerShield
    {
		public PlayerGhostShield(LevelEntity owner)
            : base(owner)
		{
            TFGame.Log(new Exception("new player ghost shield"), false);
    //        this.owner = owner;
    //        this.sprite = TFGame.SpriteData.GetSpritePartInt("Shield");
    //        if (owner is PlayerGhost)
    //        {
				//PlayerGhost ghost = owner as PlayerGhost;
				//this.sprite.Color = ArcherData.GetColorB(ghost.PlayerIndex, ghost.Allegiance);
				//if (ghost.Allegiance != Allegiance.Neutral)
     //           {
					//this.particleType = Particles.TeamDash[(int)ghost.Allegiance];
     //           }
     //           else
     //           {
					//this.particleType = Particles.Dash[ghost.PlayerIndex];
            //    }
            //}
            //else
            //{
            //    this.sprite.Color = ArcherData.Enemies.ColorB;
            //    this.particleType = Particles.TeamDash[1];
            //}
            //this.sprite.Play(0, false);
            //base.Add(this.sprite);
            //this.sine = new SineWave(120);
            //base.Add(this.sine);
        }
    }
}
