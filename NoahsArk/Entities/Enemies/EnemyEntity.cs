using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities.Sprites;
using NoahsArk.Rendering;

namespace NoahsArk.Entities.Enemies
{
    public class EnemyEntity
    {
        #region Fields
        private EEnemyType _enemyType;
        private float _maxHealthPoints;
        private float _maxManaPoints;
        private float _experienceRewardPoints;
        private float _speed;
        private Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> _animations;
        private Texture2D _shadow;
        private Texture2D _rarityMarker;
        private Camera _camera;
        private ERarity _rarityType;
        private List<LootDrop> _lootTable;
        #endregion

        #region Properties
        public EEnemyType EnemyType { get { return _enemyType; } }
        public float MaxHealthPoints { get { return _maxHealthPoints; } }
        public float MaxManaPoints { get { return _maxManaPoints; } } 
        public float ExperienceRewardPoints { get { return _experienceRewardPoints; } }
        public float Speed { get {  return _speed; } }
        public Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> Animations { get { return _animations; } }
        public Texture2D Shadow { get { return _shadow; } }
        public Texture2D RarityMarker { get { return _rarityMarker; } }
        public Camera Camera { get { return _camera; } }
        public ERarity RarityType { get { return _rarityType; } }
        public List<LootDrop> LootTable { get { return _lootTable; } }
        #endregion

        #region Constructor
        public EnemyEntity(EEnemyType enemyType, int maxHealthPoints, int maxManaPoints, float experienceRewardPoints, float speed, ERarity rarityType,
            Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> animations, 
            Camera camera, Texture2D rarityMarker, Texture2D shadow = null, List<LootDrop> lootTable = null)
        {
            _enemyType = enemyType;
            _maxHealthPoints = maxHealthPoints;
            _maxManaPoints = maxManaPoints;
            _experienceRewardPoints = experienceRewardPoints;
            _speed = speed;
            _rarityType = rarityType;
            _animations = animations;
            _camera = camera;
            _shadow = shadow;
            _rarityMarker = rarityMarker;
            _lootTable = lootTable;
        }
        #endregion
    }
}
