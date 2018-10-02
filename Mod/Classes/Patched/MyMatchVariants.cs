using TowerFall;
using Patcher;

namespace Mod
{
	[Patch]
	public class MyMatchVariants : MatchVariants
	{
		[Header("MODS")]
		[PerPlayer, CanRandom]
		public Variant NoHeadBounce;
		[PerPlayer, CanRandom]
		public Variant NoLedgeGrab;
		[CanRandom]
		public Variant AwfullySlowArrows;
		[CanRandom]
		public Variant AwfullyFastArrows;
		[PerPlayer, CanRandom]
		public Variant InfiniteArrows;
		[PerPlayer, CanRandom]
		public Variant NoDodgeCooldowns;

		[PerPlayer, CanRandom]
		public Variant VarietyPack;

		[CanRandom]
		public Variant GottaBustGhosts;

		[CanRandom]
		public Variant CrownSummonsChaliceGhost;

		[CanRandom]
		public Variant ChaliceGhostsHuntGhosts;

		[CanRandom]
		public Variant FastGhosts;

		[CanRandom]
		public Variant GhostRevives;

		[CanRandom]
        public Variant GhostItems;

        [CanRandom]
        public Variant GhostJoust;

		[Description ("NEW RANDOM VARIANTS EVERY ROUND")]
		public Variant CalvinFall;

		public MyMatchVariants(bool noPerPlayer = false) : base(noPerPlayer)
		{
			// mutually exclusive variants
			this.CreateLinks(NoHeadBounce, NoTimeLimit);
			this.CreateLinks(NoDodgeCooldowns, ShowDodgeCooldown);
			this.CreateLinks(AwfullyFastArrows, AwfullySlowArrows);

			this.FreeAiming.TournamentRules = true;
			this.FreeAiming.TeamTournamentRules = true;
		}
	}
}
