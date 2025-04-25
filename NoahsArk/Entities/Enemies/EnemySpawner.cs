using System;
using Microsoft.Xna.Framework;
using NoahsArk.Levels;
using NoahsArk.States;

namespace NoahsArk.Entities.Enemies
{
    public class EnemySpawner
    {
        #region Fields
        private EEnemyType _enemyType;
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
        #endregion

        #region Constructor
        public EnemySpawner(Vector2 position, float width, float height, EEnemyType enemyType, int maxSpawnCount)
        {
            _maxSpawnRadius = 100f;
            _spawnBounds = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
            _enemyType = enemyType;
            _maxSpawnCount = maxSpawnCount;
            _random = new Random();
            _isReadyToSpawn = false;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > _nextSpawnTime)
            {
                _isReadyToSpawn = true;
                _nextSpawnTime = (float)(gameTime.TotalGameTime.TotalSeconds + _random.Next(5, 10));
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
            if (GamePlayScreen.EnemyEntityDict.TryGetValue(EnemyType, out EnemyEntity e) &&
                e != null)
            {                
                Vector2 spawnPosition = GetRandomSpawnPosition();
                Enemy enemy = new Enemy(e.MaxHealthPoints, e.MaxManaPoints, spawnPosition, e.Speed, e.Animations, e.Shadow, e.Camera, e.IAIBehavior);
                currentMap.AddEnemy(_enemyType, enemy);
            }
            
        }
        #endregion
    }
}
