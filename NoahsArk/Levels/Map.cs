using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities;
using NoahsArk.Entities.Enemies;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Levels.Maps;
using NoahsArk.Managers;
using NoahsArk.Rendering;

namespace NoahsArk.Levels
{
    public class Map
    {
        #region Fields
        private EMapCode _mapCode;
        private TileMap _tileMap;
        private List<Player> _players = new List<Player>();
        private Dictionary<EEnemyType, Dictionary<ERarity, List<Enemy>>> _enemies = new Dictionary<EEnemyType, Dictionary<ERarity, List<Enemy>>>();
        private List<Entity> _entities = new List<Entity>();
        private List<FloatingText> _floatingTexts = new List<FloatingText>();
        private Texture2D _debugTexture;
        private Game1 _gameRef;
        #endregion

        #region Properties
        public List<Player> Players { get { return _players; } }
        public TileMap TileMap { get { return _tileMap; } }
        public Dictionary<EEnemyType, Dictionary<ERarity, List<Enemy>>> Enemies {  get { return _enemies; } }    
        public List<Entity> Entities { get { return _entities; } }
        public Game1 GameRef { get { return _gameRef; } }   
        #endregion

        #region Constructor
        public Map(Game1 gameRef, TileMap tileMap, Texture2D debugTexture)
        {
            _gameRef = gameRef;
            _tileMap = tileMap;
            _debugTexture = debugTexture;
            ERarity[] rarities = Enum.GetValues(typeof(ERarity))
                .Cast<ERarity>()
                .ToArray();
            for (int i = 0; i < _tileMap.EnemySpawners.Count; i++)
            {
                EnemySpawner enemySpawner = _tileMap.EnemySpawners[i];
                _enemies[enemySpawner.EnemyType] = new Dictionary<ERarity, List<Enemy>>();
                for (int j = 0; j < rarities.Length; j++)
                {
                    ERarity r = rarities[j];
                    _enemies[enemySpawner.EnemyType][r] = new List<Enemy>();
                }
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
                    _enemies[enemySpawner.EnemyType].TryGetValue(enemySpawner.Rarity, out List<Enemy> enemies) &&
                    enemies != null &&
                    enemySpawner.MaxSpawnCount > enemies.Count)
                {
                    // spawn enemies until we reach the max spawn count
                    while (enemies.Count < enemySpawner.MaxSpawnCount)
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

            for (int i = 0; i < _floatingTexts.Count; i++)
            {
                _floatingTexts[i].Update(gameTime);
                if (_floatingTexts[i].Lifetime <= 0)
                {
                    _floatingTexts.RemoveAt(i);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            DrawLayersBeforeCharacter(spriteBatch, gameTime, camera, out List<ILayer> layersToDrawAfterCharacter);
            DrawEntities(spriteBatch);
            DrawLayersAfterCharacter(spriteBatch, gameTime, camera, layersToDrawAfterCharacter);
            DrawFloatingTexts(spriteBatch);
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
        public void AddEnemy(Enemy enemy)
        {
            _enemies[enemy.EnemyType][enemy.Rarity].Add(enemy);
            _entities.Add(enemy);
            enemy.CurrentMap = this;
        }
        public void RemoveEnemy(Enemy enemy)
        {
            _enemies[enemy.EnemyType][enemy.Rarity].Remove(enemy);
            _entities.Remove(enemy);
            enemy.CurrentMap = null;    
        }
        public void AddFloatingText(string text, Vector2 position, Color color, float lifetime, int size)
        {
            FloatingText ft = new FloatingText(text, position, new Vector2(0, -10),
                color, lifetime, ControlManager.SpriteFont("Monogram", size));
            _floatingTexts.Add(ft);
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
        private void DrawFloatingTexts(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _floatingTexts.Count; i++)
            {
                _floatingTexts[i].Draw(spriteBatch);
            }
        }
        #endregion
    }
}
