using System.Collections.Generic;
using Newtonsoft.Json;

namespace NoahsArk.Entities.Players
{
    public class PlayerData
    {
        [JsonProperty("playerData")]
        public List<PlayerObject> PlayerObjects { get; set; }
    }
}
