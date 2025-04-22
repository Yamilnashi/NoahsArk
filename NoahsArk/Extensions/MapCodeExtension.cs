using NoahsArk.Levels;

namespace NoahsArk.Extensions
{
    public static class MapCodeExtension
    {
        public static string TileMapFilePath(this EMapCode mapCode)
        {
            string fileName = GetTileMapFileName(mapCode);
            return $"Content/Tiled/GameMaps/{fileName}";
        }

        #region Private
        private static string GetTileMapFileName(EMapCode mapCode)
        {
            return mapCode switch
            {
                EMapCode.Development => "home.tmx",
                _ => throw new System.Exception($"Missing TileMap file for map code: {mapCode}")
            };
        }
        #endregion
    }
}
