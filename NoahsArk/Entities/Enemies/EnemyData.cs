using System.Collections.Generic;
using Newtonsoft.Json;

namespace NoahsArk.Entities.Enemies
{
    public class EnemyData
    {
        [JsonProperty("enemyData")]
        public List<EnemyObject> EnemyObjects { get; set; }
    }
}
