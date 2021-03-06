using Patcher;
using TowerFall;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System;

namespace Mod
{
    [Patch]
    public class MyPlayerCorpse : PlayerCorpse
    {
        public bool reviverAdded;
        public bool spawningGhost;
        public bool hasGhost;

        public MyPlayerCorpse (PlayerCorpse.EnemyCorpses enemyCorpse, Vector2 position, Facing facing, int killerIndex) : base (enemyCorpse.ToString (), Allegiance.Neutral, position, facing, -1, killerIndex)
        {
        }

        public override void Update ()
        {
            base.Update();
            if (this.reviverAdded == true) {
                this.reviverAdded = false;
                List<Entity> teamRevivers = base.Level[GameTags.TeamReviver];
                for (int i = 0; i < teamRevivers.Count; i++) {
                    TeamReviver teamReviver = (TeamReviver)(teamRevivers[i]);
                    if (teamReviver.Corpse.PlayerIndex == this.PlayerIndex) {
                      base.Level.Layers[teamReviver.LayerIndex].Remove(teamReviver);
                      MyTeamReviver myTeamReviver = new MyTeamReviver(
                        this,
                        TeamReviver.Modes.TeamDeathmatch,
                        ((MyTeamDeathmatchRoundLogic)(base.Level.Session.RoundLogic)).GetRoundEndCounter(),
                        ((MyMatchVariants)(base.Level.Session.MatchSettings.Variants)).GhostRevives
                      );
                      base.Level.Layers[myTeamReviver.LayerIndex].Add(myTeamReviver, false);
                    }
                }
            }
        }

        public override void Added ()
        {
            this.spawningGhost = true;
            this.hasGhost = false;
            base.Added();

            if (this.PlayerIndex != -1) {
                if (base.Level.Session.MatchSettings.Mode == Modes.TeamDeathmatch && base.Level.Session.MatchSettings.Variants.TeamRevive) {
                    reviverAdded = true;
                }
            }
        }

        public override void DieByArrow (Arrow arrow, int ledge)
        {
            if (this.CanDoPrismHit (arrow)) {
                this.spawningGhost = false;
            }
            base.DieByArrow(arrow, ledge);
        }
    }
}
