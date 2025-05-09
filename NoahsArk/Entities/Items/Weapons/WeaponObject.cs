using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;

namespace NoahsArk.Entities.Items.Weapons
{
    public class WeaponObject : Weapon
    {
        #region Fields
        private string _name;
        private EWeaponType _weaponType;
        private EMaterialType _materialType;
        private float _attackHitboxOffset;
        private WeaponStats _baseStats;
        private Dictionary<EAnimationKey, Dictionary<EDirection, string>> _animations;
        #endregion

        #region Properties
        [JsonProperty("name")]
        public string Name { get { return _name; }  set { _name = value; } }
        [JsonProperty("weaponType")]
        public EWeaponType WeaponType { get { return _weaponType; } set { _weaponType = value; } }
        [JsonProperty("materialType")]
        public EMaterialType MaterialType { get { return _materialType; } set { _materialType = value; } }
        [JsonProperty("attackHitboxOffset")]
        public float AttackHitboxOffset { get { return _attackHitboxOffset; } set { _attackHitboxOffset = value; } }
        [JsonProperty("baseStats")]
        public WeaponStats BaseStats { get { return _baseStats; } set { _baseStats = value; } }
        [JsonProperty("animations")]
        public Dictionary<EAnimationKey, Dictionary<EDirection, string>> Animations { get { return _animations; } set { _animations = value; } }
        #endregion

        #region Constructor
        public WeaponObject() { }

        public override float CalculateDamage(out bool isCrit)
        {
            isCrit = false;
            float minimumDamage = _baseStats.MinimumDamage;
            float maximumDamage = _baseStats.MaximumDamage;
            float critChance = _baseStats.CriticalStrikeChance;
            float critMultiplier = _baseStats.CriticalStrikeDamage;
            float baseDamage = minimumDamage + (float)Random.NextDouble() * (maximumDamage - minimumDamage);

            // roll randomly
            float roll = (float)Random.NextDouble();
            isCrit = roll < critChance;

            if (isCrit)
            {
                return baseDamage * (1 + critMultiplier);
            }

            return baseDamage;
        }
        #endregion

        #region Methods
        #endregion
    }
}
