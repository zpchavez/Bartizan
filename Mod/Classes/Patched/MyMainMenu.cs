using System;
using Monocle;
using Patcher;
using TowerFall;

namespace Mod
{
	[Patch]
	public class MyMainMenu : MainMenu
    {
		public MyMainMenu(MenuState state) : base(state)
        {
        }

		public new static void PlayMenuMusic(bool fromArchives = false, bool forceNormal = false)
        {
			forceNormal = false;
            if (!(Music.CurrentSong == "AltTitle") && !(Music.CurrentSong == "ChipTitle") && !(Music.CurrentSong == "Title"))
            {
                if (forceNormal)
					{
                    Music.Play("AltTitle");
                    MainMenu.wasChipTheme = false;
                }
                else if (fromArchives)
                {
                    if (MainMenu.wasChipTheme)
                    {
                        Music.Play("ChipTitle");
                    }
                    else
                    {
                        Music.Play("AltTitle");
                    }
                }
                else if (Calc.Chance(Calc.Random, 0.5f))
                {
                    Music.Play("ChipTitle");
                    MainMenu.wasChipTheme = true;
                }
                else
                {
                    Music.Play("AltTitle");
                    MainMenu.wasChipTheme = false;
                }
            }
        }
    }
}
