using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities;
using NoahsArk.Entities.Enemies;
using NoahsArk.Levels.Maps;
using NoahsArk.Rendering;

namespace NoahsArk.Levels
{
    public class Map
    {
        #region Fields
        private EMapCode _mapCode;
        private TileMap _tileMap;
        private List<Player> _players = new List<Player>();
        private Dictionary<EEnemyType, List<Enemy>> _enemies = new Dictionary<EEnemyType, List<Enemy>>();
        private List<Entity> _entities = new List<Entity>();
        private Texture2D _debugTexture;
        // characters
        // enemies
        #endregion

        #region Properties
        public List<Player> Players { get { return _players; } }
        public TileMap TileMap { get { return _tileMap; } }
        public Dictionary<EEnemyType, List<Enemy>> Enemies {  get { return _enemies; } }    
        public List<Entity> Entities { get { return _entities; } }
        #endregion

        #region Constructor
        public Map(TileMap tileMap, Texture2D debugTexture)
        {
            _tileMap = tileMap;
            _debugTexture = debugTexture;
            for (int i = 0; i < _tileMap.EnemySpawners.Count; i++)
            {
                EnemySpawner enemySpawner = _tileMap.EnemySpawners[i];
                _enemies[enemySpawner.EnemyType] = new List<Enemy>();
            }
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _tileMap.EnemySpawners.Count; i++)
            {
                EnemySpawner enemySpawner = _tileMap.EnemySpawners[i];
                enemySpawner.Update(gameTime);
                
                if (enemySpawner.IsReadyToSpawn && 
                    enemySpawner.MaxSpawnCount > _enemies[enemySpawner.EnemyType].Count)
                {
                    // spawn enemies until we reach the max spawn count
                    while (_enemies[enemySpawner.EnemyType].Count < enemySpawner.MaxSpawnCount)
                    {
                        enemySpawner.SpawnEnemy(this);
                    }
                    enemySpawner.IsReadyToSpawn = false;
                }
            }
            for (int i = 0; i < _entities.Count; i++)
            {
                Entity entity = _entities[i];
                entity.Update(gameTime);
            }

            for (int i = 0; i < _tileMap.MapLayers.Count; i++)
            {
                ILayer layer = _tileMap.MapLayers[i];
                layer.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            DrawLayersBeforeCharacter(spriteBatch, gameTime, camera, out List<ILayer> layersToDrawAfterCharacter);
            DrawEntities(spriteBatch);
            DrawLayersAfterCharacter(spriteBatch, gameTime, camera, layersToDrawAfterCharacter);
        }
        public void AddPlayer(Player player)
        {
            _players.Add(player);
            _entities.Add(player);
            player.CurrentMap = this;
        }
        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            _entities.Remove(player);
            player.CurrentMap = null;
        }
        public void AddEnemy(EEnemyType enemyType, Enemy enemy)
        {
            _enemies[enemyType].Add(enemy);
            _entities.Add(enemy);
            enemy.CurrentMap = this;
        }
        public void RemoveEnemy(EEnemyType enemyType, Enemy enemy)
        {
            _enemies[enemyType].Remove(enemy);
            _entities.Remove(enemy);
            enemy.CurrentMap = null;    
        }

        #endregion
        #region Private
        private void DrawLayersBeforeCharacter(SpriteBatch spriteBatch, GameTime gameTime, Camera camera,out List<ILayer> layersToDrawAfterCharacter)
        {
            layersToDrawAfterCharacter = new List<ILayer>();
            for (int i = 0; i < _tileMap.MapLayers.Count; i++)
            {
                ILayer layer = _tileMap.MapLayers[i];
                if (layer == null)
                {
                    continue;
                }

                if (layer.HasProperty("drawAfterCharacter") &&
                layer.GetProperty<bool>("drawAfterCharacter"))
                {
                    layersToDrawAfterCharacter.Add(layer);
                    continue;
                }

                layer.Draw(spriteBatch, gameTime, camera, _tileMap.TileSets);
            }
        }
        private void DrawEntities(SpriteBatch spriteBatch)
        {
            List<Entity> sortedEntities = _entities.OrderBy(x => x.GetDepthY()).ToList();
            for (int i = 0; i < sortedEntities.Count; i++)
            {
                Entity entity = sortedEntities[i];
                entity.Draw(spriteBatch);
            }
        }
        private void DrawLayersAfterCharacter(SpriteBatch spriteBatch, GameTime gameTime, Camera camera, List<ILayer> layersToDrawAfterCharacter)
        {
            for (int i = 0; i < layersToDrawAfterCharacter.Count; i++)
            {
                ILayer layer = layersToDrawAfterCharacter[i];
                layer.Draw(spriteBatch, gameTime, camera, _tileMap.TileSets);
            }
        }
        #endregion
    }
}
