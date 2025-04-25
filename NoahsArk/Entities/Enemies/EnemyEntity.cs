using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using NoahsArk.Rendering;

namespace NoahsArk.Entities.Enemies
{
    public class EnemyEntity
    {
        #region Fields
        private IAIBehavior _iAiBehavior;
        private EEnemyType _enemyType;
        private int _maxHealthPoints;
        private int _maxManaPoints;
        private float _speed;
        private Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> _animations;
        private Texture2D _shadow;
        private Camera _camera;
        #endregion

        #region Properties
        public IAIBehavior IAIBehavior {  get { return _iAiBehavior; } }
        public EEnemyType EnemyType { get { return _enemyType; } }
        public int MaxHealthPoints { get { return _maxHealthPoints; } }
        public int MaxManaPoints { get { return _maxManaPoints; } } 
        public float Speed { get {  return _speed; } }
        public Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> Animations { get { return _animations; } }
        public Texture2D Shadow { get { return _shadow; } }
        public Camera Camera { get { return _camera; } }
        #endregion

        #region Constructor
        public EnemyEntity(EEnemyType enemyType, int maxHealthPoints, int maxManaPoints, float speed,
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations, 
            Camera camera, IAIBehavior aiBehavior, Texture2D shadow = null)
        {
            _enemyType = enemyType;
            _maxHealthPoints = maxHealthPoints;
            _maxManaPoints = maxManaPoints;
            _speed = speed;
            _animations = animations;
            _camera = camera;
            _shadow = shadow;
        }
        #endregion
    }
}
