using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
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
        private string _groundTexturePath;
        private Texture2D _groundTexture;
        private float _attackHitboxOffset;
        private WeaponStats _baseStats;
        private Dictionary<EAnimationKey, Dictionary<EDirection, string>> _animations;
        #endregion

        #region Properties
        [JsonProperty("name")]
        public override string Name { get { return _name; }  set { _name = value; } }
        [JsonProperty("weaponType")]
        public EWeaponType WeaponType { get { return _weaponType; } set { _weaponType = value; } }
        [JsonProperty("materialType")]
        public EMaterialType MaterialType { get { return _materialType; } set { _materialType = value; } }
        [JsonProperty("groundTexturePath")]
        public string GroundTexturePath { get { return _groundTexturePath; } set { _groundTexturePath = value; }}
        [JsonProperty("attackHitboxOffset")]
        public float AttackHitboxOffset { get { return _attackHitboxOffset; } set { _attackHitboxOffset = value; } }
        [JsonProperty("baseStats")]
        public WeaponStats BaseStats { get { return _baseStats; } set { _baseStats = value; } }
        [JsonProperty("animations")]
        public Dictionary<EAnimationKey, Dictionary<EDirection, string>> Animations { get { return _animations; } set { _animations = value; } }
        [JsonIgnore]
        public Texture2D GroundTexture { get { return _groundTexture; } set { _groundTexture = value; } }
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
