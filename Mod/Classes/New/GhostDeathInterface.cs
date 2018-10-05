using System;
using TowerFall;
namespace Mod
{
    public interface GhostDeathInterface
    {
        void OnPlayerGhostDeath(PlayerGhost ghost, PlayerCorpse corpse);
    }
}
