using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NoahsArk.Entities.Behaviors;
using NoahsArk.Levels;
using NoahsArk.States;

namespace NoahsArk.Entities.Enemies
{
    public class EnemySpawner
    {
        #region Fields
        private EEnemyType _enemyType;
        private ERarity _rarity;
        private int _maxSpawnCount;
        private float _nextSpawnTime;
        private float _maxSpawnRadius;
        private Vector2 _spawnPosition;
        private Random _random;
        private Rectangle _spawnBounds;
        private bool _isReadyToSpawn;
        #endregion

        #region Properties
        public EEnemyType EnemyType { get { return _enemyType; } }
        public int MaxSpawnCount {  get { return _maxSpawnCount; } }    
        public bool IsReadyToSpawn {  get { return _isReadyToSpawn; } set { _isReadyToSpawn = value; } } 
        public ERarity Rarity { get {  return _rarity; } }
        #endregion

        #region Constructor
        public EnemySpawner(Vector2 position, float width, float height, EEnemyType enemyType, ERarity rarity, int maxSpawnCount)
        {
            _maxSpawnRadius = 100f;
            _spawnBounds = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
            _enemyType = enemyType;
            _maxSpawnCount = maxSpawnCount;
            _random = new Random();
            _isReadyToSpawn = false;
            _rarity = rarity;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > _nextSpawnTime)
            {
                _isReadyToSpawn = true;
                _nextSpawnTime = (float)(gameTime.TotalGameTime.TotalSeconds + _random.Next(40, 45));
            }
        }
        public Vector2 GetRandomSpawnPosition()
        {
            float angle = (float)(_random.NextDouble() * Math.PI * 2);
            float radius = (float)(_random.NextDouble() * _maxSpawnRadius);
            float x = _spawnBounds.X + (float)Math.Cos(angle) * radius;
            float y = _spawnBounds.Y + (float)Math.Sin(angle) * radius;
            return new Vector2(x, y);
        }
        public void SpawnEnemy(Map currentMap)
        {
            if (GamePlayScreen.EnemyEntityDict.TryGetValue(EnemyType, out Dictionary<ERarity, EnemyEntity> entityDict) &&
                entityDict != null)
            {   
                if (entityDict.TryGetValue(_rarity, out EnemyEntity e))
                {
                    Vector2 spawnPosition = GetRandomSpawnPosition();
                    Enemy enemy = new Enemy(e.EnemyType, e.MaxHealthPoints, e.MaxManaPoints, spawnPosition, e.Speed, e.RarityType, e.Animations,
                        e.Shadow, e.RarityMarker, e.Camera, new PatrolAI());
                    currentMap.AddEnemy(enemy);
                }             
            }
            
        }
        #endregion
    }
}
