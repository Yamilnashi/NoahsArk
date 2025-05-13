using System;
using NoahsArk.Entities.GameObjects;

namespace NoahsArk.Extensions
{
    public static class MenuCategoryTypeExtensions
    {
        public static (string activeIconFilePath, string inactiveIconFilePath) GetIconFilePath(this EMenuCategoryType category)
        {
            (string activeIconFilePath, string inactiveIconFilePath) paths = category switch
            {
                EMenuCategoryType.Equipment => ("menu-category-chest", "menu-category-inactive-chest"),
                EMenuCategoryType.Character => ("menu-category-swords", "menu-category-inactive-swords"),
                EMenuCategoryType.Map => ("menu-category-stats", "menu-category-inactive-stats"),
                EMenuCategoryType.Achievements => ("menu-category-heart", "menu-category-inactive-heart"),
                EMenuCategoryType.Options => ("menu-category-wrench", "menu-category-inactive-wrench"),
                _ => throw new Exception($"Menu Category type not found: {category}")
            };
            return ($"Assets/Menus/{paths.activeIconFilePath}", $"Assets/Menus/{paths.inactiveIconFilePath}");
        }
    }
}
