using Newtonsoft.Json;

namespace NoahsArk.Entities.Items.Weapons
{
    public class WeaponStats
    {
        #region Fields        
        private float _minimumDamage;
        private float _maximumDamage;
        private float _attackSpeed;        
        private float _criticalStrikeChance;        
        private float _criticalStrikeDamage;
        #endregion

        #region Properties
        [JsonProperty("minimumDamage")]
        public float MinimumDamage { get { return _minimumDamage; } set { _minimumDamage = value; } }
        [JsonProperty("maximumDamage")]
        public float MaximumDamage { get { return _maximumDamage; } set { _maximumDamage = value; } }   
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
