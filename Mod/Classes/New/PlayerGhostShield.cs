using System;
using Microsoft.Xna.Framework;
using Mod;
using Monocle;
using TowerFall;

namespace New
{
    public class PlayerGhostShield : CompositeComponent
    {
        public LevelEntity owner;

        public SpritePart<int> sprite;

        public SineWave sine;

        public ParticleType particleType;

        public PlayerGhostShield(LevelEntity owner) :base(false, false)
        {
            this.owner = owner;
            this.sprite = TFGame.SpriteData.GetSpritePartInt("Shield");
            if (owner is PlayerGhost)
            {
                PlayerGhost ghost = owner as PlayerGhost;
                this.sprite.Color = ArcherData.GetColorB(ghost.PlayerIndex, ghost.Allegiance);
                if (ghost.Allegiance != Allegiance.Neutral)
                {
                    this.particleType = Particles.TeamDash[(int)ghost.Allegiance];
                }
                else
                {
                    this.particleType = Particles.Dash[ghost.PlayerIndex];
                }
            }
            else
            {
                this.sprite.Color = ArcherData.Enemies.ColorB;
                this.particleType = Particles.TeamDash[1];
            }
            this.sprite.Play(0, false);
            base.Add(this.sprite);
            this.sine = new SineWave(120);
            base.Add(this.sine);
        }

        public override void Update()
        {
            this.sprite.Scale.X = 0.7f + this.sine.Value * 0.1f;
            this.sprite.Scale.Y = 0.7f - this.sine.Value * 0.1f;
            base.Update();
        }

        public void Gain()
        {
            base.Active = (base.Visible = true);
            this.sprite.DrawHeight = 0f;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.Invert(Ease.CubeInOut), 30, true);
            tween.OnUpdate = delegate (Tween t) {
                this.sprite.DrawHeight = t.Eased;
            };
            base.Add(tween);
            Sounds.pu_shield.Play(base.Entity.X, 1f);
        }

        public void Lose()
        {
            base.Active = (base.Visible = false);
            for (int i = 0; i < 360; i += 15)
            {
                Vector2 value = Calc.AngleToVector((float)i, 12f);
                (base.Entity.Scene as Level).Particles.Emit(this.particleType, base.Entity.Position + value);
            }
            Sounds.pu_shieldImp.Play(base.Entity.X, 1f);
        }
    }
}
