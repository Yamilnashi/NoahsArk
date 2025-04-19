using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using NoahsArk.Levels;
using NoahsArk.Rendering;

namespace NoahsArk.Entities
{
    public abstract class Entity
    {
        #region Fields
        private int _healthPoints;
        private int _maxHealthPoints;
        private int _manaPoints;
        private int _maxManaPoints;
        private int _experiencePoints;
        private float _speed;
        private Inventory _inventory;
        private Dictionary<EEquipmentSlot, Item> _equippedItems;
        private Vector2 _position;
        private Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> _animations;
        private EAnimationKey _currentAnimationKey;
        private EDirection _currentDirection;
        private Map _currentMap;
        private Camera _camera;
        #endregion

        #region Properties
        public int HealthPoints {  get { return _healthPoints; } protected set { _healthPoints = value; } }
        public int MaxHealthPoints { get { return _maxHealthPoints; } protected set { _maxHealthPoints = value; } }
        public int ManaPoints { get { return _manaPoints; } protected set { _manaPoints = value; } }
        public int MaxManaPoints { get { return _maxManaPoints; } protected set { _maxManaPoints = value; } }
        public int ExperiencePoints { get { return _experiencePoints; } protected set { _experiencePoints = value; } }
        public float Speed { get { return _speed; } protected set { _speed = value; } }
        public Inventory Inventory { get { return _inventory; } protected set { _inventory = value; } } 
        public Dictionary<EEquipmentSlot, Item> EquippedItems { get { return _equippedItems; } protected set { _equippedItems = value; } }
        public Vector2 Position { get { return _position; } protected set { _position = value; } }
        public EDirection CurrentDirection { get { return _currentDirection; } }
        public Map CurrentMap { get { return _currentMap; } set { _currentMap = value; } }
        public Camera Camera { get { return _camera; } }
        #endregion

        #region Constructor
        public Entity(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed,
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations, Camera camera)
        {
            _maxHealthPoints = maxHealthPoints;
            _healthPoints = maxHealthPoints;
            _maxManaPoints = maxManaPoints;
            _manaPoints = maxManaPoints;
            _experiencePoints = 0;
            _speed = speed;
            _inventory = new Inventory();
            _equippedItems = new Dictionary<EEquipmentSlot, Item>();
            _position = initialPosition;
            _animations = animations;
            _currentAnimationKey = EAnimationKey.Idle;
            _currentDirection = EDirection.Down;
            _camera = camera;
        }
        #endregion

        #region Methods
        public virtual void Update(GameTime gameTime)
        {
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    _animations[_currentAnimationKey][_currentDirection].Update(gameTime, _currentAnimationKey);
                }
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    _animations[_currentAnimationKey][_currentDirection].Draw(spriteBatch, _position, _currentDirection);
                }
            }
        }
        public void SetAnimation(EAnimationKey key, EDirection direction)
        {
            if (_animations.ContainsKey(key))
            {
                _currentAnimationKey = key;
                if (_animations[key].ContainsKey(_currentDirection))
                {
                    if (direction != _currentDirection)
                    {
                        _animations[key][direction].Reset();
                    }                    
                    _currentDirection = direction;                                     
                }                             
            }
        }
        public void TakeDamage(int amount)
        {
            _healthPoints = MathHelper.Max(0, _healthPoints - amount);
        }
        public void DealDamage(Entity target)
        {
            int damage = CalculateDamage();
            target.TakeDamage(damage);
        }
        public void Move(Vector2 direction)
        {
            _position += direction; // todo: collisions
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    _animations[_currentAnimationKey][_currentDirection].UpdatePosition(_position);
                }                
            }
        }
        protected virtual int CalculateDamage()
        {
            // todo: customize calculation based on weapon, armor, stats, type, etc
            return 10;
        }
        public void LockToMap()
        {
            _position.X = MathHelper.Clamp(_position.X, 0, _currentMap.TileMap.MapWidth - _animations[_currentAnimationKey][_currentDirection].FrameWidth);
            _position.Y = MathHelper.Clamp(_position.Y, 0, _currentMap.TileMap.MapHeight - _animations[_currentAnimationKey][_currentDirection].FrameHeight);
        }
        #endregion
    }
}
