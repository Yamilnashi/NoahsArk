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
        private EEnemyType _enemyType;
        private int _maxHealthPoints;
        private int _maxManaPoints;
        private float _speed;
        private Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> _animations;
        private Texture2D _shadow;
        private Texture2D _rarityMarker;
        private Camera _camera;
        private ERarity _rarityType;
        #endregion

        #region Properties
        public EEnemyType EnemyType { get { return _enemyType; } }
        public int MaxHealthPoints { get { return _maxHealthPoints; } }
        public int MaxManaPoints { get { return _maxManaPoints; } } 
        public float Speed { get {  return _speed; } }
        public Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> Animations { get { return _animations; } }
        public Texture2D Shadow { get { return _shadow; } }
        public Texture2D RarityMarker { get { return _rarityMarker; } }
        public Camera Camera { get { return _camera; } }
        public ERarity RarityType { get { return _rarityType; } }
        #endregion

        #region Constructor
        public EnemyEntity(EEnemyType enemyType, int maxHealthPoints, int maxManaPoints, float speed, ERarity rarityType,
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> animations, 
            Camera camera, Texture2D rarityMarker, Texture2D shadow = null)
        {
            _enemyType = enemyType;
            _maxHealthPoints = maxHealthPoints;
            _maxManaPoints = maxManaPoints;
            _speed = speed;
            _rarityType = rarityType;
            _animations = animations;
            _camera = camera;
            _shadow = shadow;
            _rarityMarker = rarityMarker;
        }
        #endregion
    }
}
