using System.Collections.Generic;
using Newtonsoft.Json;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;

namespace NoahsArk.Entities.Players
{
    public class PlayerObject
    {
        [JsonProperty("classType")]
        public EClassType ClassType { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("healthPoints")]
        public int HealthPoints { get; set; }
        [JsonProperty("manaPoints")]
        public int ManaPoints { get; set; }
        [JsonProperty("speed")]
        public float Speed { get; set; }
        [JsonProperty("animations")]
        public Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> Animations { get; set; }
    }
}
