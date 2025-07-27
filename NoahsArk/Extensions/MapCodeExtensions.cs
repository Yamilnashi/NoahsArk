using System.Numerics;
using NoahsArk.Levels;

namespace NoahsArk.Extensions
{
    public static class MapCodeExtensions
    {
        public static string TileMapFilePath(this EMapCode mapCode)
        {
            string fileName = GetTileMapFileName(mapCode);
            return $"Content/Tiled/GameMaps/{fileName}";
        }

        public static Vector2 GetInitialPosition(this EMapCode mapCode)
        {
            return mapCode switch
            {
                EMapCode.Act1 => new Vector2(835, 828),
                _ => new Vector2(295, 240)
            };
        }

        #region Private
        private static string GetTileMapFileName(EMapCode mapCode)
        {
            return mapCode switch
            {
                EMapCode.Development => "development.tmx",
                EMapCode.Obstacles => "obstacles.tmx",
                EMapCode.Test => "test.tmx",
                EMapCode.HomeInside => "homeinside.tmx",
                EMapCode.HomeOutside => "homeoutside.tmx",
                EMapCode.Restaurant => "restaurant.tmx",
                EMapCode.Act1 => "act-1.tmx",
                _ => throw new System.Exception($"Missing TileMap file for map code: {mapCode}")
            };
        }
        #endregion
    }
}
