using NoahsArk.Entities;

namespace NoahsArk.Extensions
{
    public static class RarityTypeExtensions
    {
        public static int GetRarityMarkerCount(this ERarity rarity)
        {
            return rarity switch
            {
                ERarity.Normal => 0,
                ERarity.Magic => 1,
                ERarity.Rare => 2,
                ERarity.Epic => 3,
                ERarity.Legendary => 4,
                _ => 0
            };
        }
    }
}
