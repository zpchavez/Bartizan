using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TowerFall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Patcher;
using Monocle;

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
		public Variant FastGhosts;

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