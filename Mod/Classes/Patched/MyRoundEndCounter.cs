using Monocle;
using TowerFall;

namespace Mod
{
	public class MyRoundEndCounter : RoundEndCounter
	{
        // Override to make publicly accessible
		public override void Reset ()
		{
            base.Reset();
		}
	}
}
