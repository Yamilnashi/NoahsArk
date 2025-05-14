using System;
using NoahsArk.Entities.Menus;

namespace NoahsArk.Extensions
{
    public static class MenuCategoryTypeExtensions
    {
        public static string GetIconFilePath(this EMenuCategoryType category)
        {
            string fileName = category switch
            {
                EMenuCategoryType.Equipment => "menu-category-chest",
                EMenuCategoryType.Character => "menu-category-swords",
                EMenuCategoryType.Map => "menu-category-stats",
                EMenuCategoryType.Achievements => "menu-category-heart",
                EMenuCategoryType.Options => "menu-category-wrench",
                _ => throw new Exception($"Menu Category type not found: {category}")
            };
            return $"Assets/Menus/{fileName}";
        }
    }
}
