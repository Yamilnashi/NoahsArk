using Newtonsoft.Json;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using System.Collections.Generic;

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
        [JsonProperty("speed")]
        public float Speed { get; set; }
        [JsonProperty("rarityType")]
        public ERarity RarityType { get; set; }
        [JsonProperty("animations")]
        public Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> Animations { get; set; }
    }
}
