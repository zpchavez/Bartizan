using Patcher;
using TowerFall;
using System;

namespace Mod
{
  [Patch]
	public class MyMatchSettings : MatchSettings
	{
		public MyMatchSettings(LevelSystem levelSystem, Modes mode, MatchSettings.MatchLengths matchLength)
			: base(levelSystem, mode, matchLength)
		{
		}

		public override int GoalScore {
			get {
				switch (this.Mode) {
					case RespawnRoundLogic.Mode:
					case MobRoundLogic.Mode:
						int goals;
						if (MyGlobals.MAX_PLAYERS == 8) {
							this.PlayerGoals(5, 8, 10, 10, 10, 10, 10);
						} else {
							this.PlayerGoals(5, 8, 10, 12);
						}
						return (int)Math.Ceiling(((float)goals * MatchSettings.GoalMultiplier[(int)this.MatchLength]));
					default:
						return base.GoalScore;
				}
			}
		}
	}
}
