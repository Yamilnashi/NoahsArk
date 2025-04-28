using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoahsArk.Entities.Enemies;
using NoahsArk.Entities.GameObjects;

namespace NoahsArk.Levels.Maps
{
    public class TileMap
    {
        #region Fields
        private List<TileSet> _tileSets;
        private List<ILayer> _mapLayers;
        private int _mapWidth;
        private int _mapHeight;
        private List<Rectangle> _obstacles;
        private List<DoorTransition> _doors;
        private List<EnemySpawner> _enemySpawners;
        #endregion

        #region Properties
        public List<TileSet> TileSets { get { return _tileSets; } set { _tileSets = value; } }
        public List<ILayer> MapLayers { get { return _mapLayers; } set { _mapLayers = value; } }
        public int MapWidth { get { return _mapWidth * Engine.TileWidth; } }
        public int MapHeight { get { return _mapHeight * Engine.TileHeight; } }
        public List<Rectangle> Obstacles { get { return _obstacles; } }
        public List<DoorTransition> Doors { get { return _doors; } }
        public List<EnemySpawner> EnemySpawners {  get { return _enemySpawners; } }
        #endregion

        #region Constructor
        public TileMap(int mapWidth, int mapHeight, int tileWidth, 
            int tileHeight, List<ILayer> mapLayers, List<TileSet> tileSets, 
            List<Rectangle> obstacles,
            List<DoorTransition> doors,
            List<EnemySpawner> enemySpawners)
        {
            _mapLayers = mapLayers;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _tileSets = tileSets;
            _obstacles = obstacles ?? new List<Rectangle>();
            _doors = doors ?? new List<DoorTransition>();
            _enemySpawners = enemySpawners ?? new List<EnemySpawner>();
        }
        #endregion
    }
}
