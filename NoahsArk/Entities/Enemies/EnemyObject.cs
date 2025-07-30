using System.Collections.Generic;
using Newtonsoft.Json;
using NoahsArk.Entities.Sprites;

namespace NoahsArk.Entities.Enemies
{
    public class EnemyObject
    {
        [JsonProperty("enemyType")]
        public EEnemyType EnemyType { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("healthPoints")]
        public int HealthPoints { get; set; }
        [JsonProperty("manaPoints")]
        public int ManaPoints { get; set; }
        [JsonProperty("experienceRewardPoints")]
        public int ExperienceRewardPoints { get; set; }
        [JsonProperty("speed")]
        public float Speed { get; set; }
        [JsonProperty("rarityType")]
        public ERarity RarityType { get; set; }
        [JsonProperty("animations")]
        public Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> Animations { get; set; }
        [JsonProperty("lootTable")]
        public List<LootDrop> LootTable { get; set; }
    }
}
