using Newtonsoft.Json;
using NoahsArk.Entities.Items;
using NoahsArk.Entities.Items.Weapons;

namespace NoahsArk.Entities
{
    public class LootDrop
    {
        #region Fields
        private EItemType _itemType;
        private EWeaponType? _weaponType;
        private EMaterialType? _materialType;
        private int _minQuantity;
        private int _maxQuantity;
        private float _dropChance;
        #endregion

        #region Properties
        [JsonProperty("itemType")]
        public EItemType ItemType { get { return _itemType; } set { _itemType = value; } }
        [JsonProperty("weaponType")]
        public EWeaponType? WeaponType { get { return _weaponType; } set { _weaponType = value; } }
        [JsonProperty("materialType")]
        public EMaterialType? MaterialType { get { return _materialType; } set { _materialType = value; } }
        [JsonProperty("dropChance")]
        public float DropChance { get { return _dropChance; } set { _dropChance = value; } }
        [JsonProperty("minQuantity")]
        public int MinQuantity { get { return _minQuantity; } set { _minQuantity = value; } }
        [JsonProperty("maxQuantity")]
        public int MaxQuantity { get { return _maxQuantity; } set { _maxQuantity = value; } }
        #endregion

        #region Constructor
        public LootDrop() { }
        #endregion
    }
}
