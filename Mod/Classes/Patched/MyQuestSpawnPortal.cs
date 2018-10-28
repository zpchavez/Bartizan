using Microsoft.Xna.Framework;
using Monocle;
using Patcher;
using TowerFall;

namespace Mod
{
    [Patch]
    public class MyQuestSpawnPortal : QuestSpawnPortal
    {
        public MyQuestSpawnPortal (Vector2 position, Vector2[] nodes)
            : base (position, nodes)
        {}

        public override void SpawnEnemy (string enemy)
        {
            if (((MyMatchVariants)base.Level.Session.MatchSettings.Variants).MeanerMonsters) {
                string[] choices = {
                    "Mole",
                    "TechnoMage",
                    "FlamingSkull",
                    "Birdman",
                    "DarkBirdman",
                    "Slime",
                    "RedSlime",
                    "BlueSlime",
                    "Bat",
                    "BombBat",
                    "SuperBombBat",
                    "Crow",
                    "Cultist",
                    "ScytheCultist",
                    "BossCultist"

                    // Spawning these enemies causes NullReferenceException for reasons I don't understand

                    // "Exploder",
                    // "EvilCrystal",
                    // "BlueCrystal",
                    // "BoltCrystal",
                    // "PrismCrystal",
                    // "Ghost",
                    // "GreenGhost",
                    // "Elemental",
                    // "GreenElemental",

                    // Exclude these skeleton enemies that shoot arrows

                    // "Skeleton",
                    // "BombSkeleton",
                    // "LaserSkeleton",
                    // "MimicSkeleton",
                    // "DrillSkeleton",
                    // "BoltSkeleton",
                    // "Jester",
                    // "BossSkeleton",
                    // "BossWingSkeleton",
                    // "WingSkeleton",
                    // "TriggerSkeleton",
                    // "PrismSkeleton"
                };
                enemy = choices[Calc.Random.Next(choices.Length)];
            }

            if (this.toSpawn.Count == 0) {
                this.sprite.Play (1, true);
            }
            this.toSpawn.Enqueue (enemy);
            this.autoDisappear = false;
        }
    }
}