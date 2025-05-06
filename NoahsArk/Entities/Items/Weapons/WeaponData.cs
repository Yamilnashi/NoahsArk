using System.Collections.Generic;
using Newtonsoft.Json;

namespace NoahsArk.Entities.Items.Weapons
{
    public class WeaponData
    {
        [JsonProperty("weaponData")]
        public List<WeaponObject> weaponObjects { get; set; }
    }
}
