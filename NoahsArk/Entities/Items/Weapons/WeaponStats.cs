using Newtonsoft.Json;

namespace NoahsArk.Entities.Items.Weapons
{
    public class WeaponStats
    {
        #region Fields        
        private float _damage;        
        private float _attackSpeed;        
        private float _criticalStrikeChance;        
        private float _criticalStrikeDamage;
        #endregion

        #region Properties
        [JsonProperty("damage")]
        public float Damage { get { return _damage; } set { _damage = value; } }
        [JsonProperty("attackSpeed")]
        public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
        [JsonProperty("criticalStrikeChance")]
        public float CriticalStrikeChance { get { return _criticalStrikeChance; } set { _criticalStrikeChance = value; } }
        [JsonProperty("criticalStrikeDamage")]
        public float CriticalStrikeDamage { get { return _criticalStrikeDamage; } set { _criticalStrikeDamage = value; } }
        #endregion

        #region Constructor
        public WeaponStats() { }
        #endregion
    }
}
