using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Sprites;
using NoahsArk.Levels;
using NoahsArk.Rendering;
using NoahsArk.States;
using NoahsArk.Utilities;

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
        private Dictionary<EAnimationKey, Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>> _animations;
        private EAnimationKey _currentAnimationKey;
        private EDirection _currentDirection;
        private Map _currentMap;
        private Camera _camera;
        private Texture2D _shadow;
        private bool _isDying = false;
        private bool _isFlashing = false;
        private float _flashTimer;
        private const float _flashDuration = 0.25f;
        private bool _isAttacking = false;
        #endregion

        #region Properties
        public int HealthPoints {  get { return _healthPoints; } protected set { _healthPoints = value; } }
        public int MaxHealthPoints { get { return _maxHealthPoints; } protected set { _maxHealthPoints = value; } }
        public int ManaPoints { get { return _manaPoints; } protected set { _manaPoints = value; } }
        public int MaxManaPoints { get { return _maxManaPoints; } protected set { _maxManaPoints = value; } }
        public int ExperiencePoints { get { return _experiencePoints; } protected set { _experiencePoints = value; } }
        public float Speed { get { return _speed; } protected set { _speed = value; } }
        public Inventory Inventory { get { return _inventory; } protected set { _inventory = value; } } 
        public Dictionary<EAnimationKey, Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>> Animations { get { return _animations; } }
        public Dictionary<EEquipmentSlot, Item> EquippedItems { get { return _equippedItems; } protected set { _equippedItems = value; } }
        public Vector2 Position { get { return _position; } protected set { _position = value; } }
        public EDirection CurrentDirection { get { return _currentDirection; } }
        public EAnimationKey CurrentAnimation { get { return _currentAnimationKey; } }
        public Map CurrentMap { get { return _currentMap; } set { _currentMap = value; } }
        public Camera Camera { get { return _camera; } }
        public bool IsDying { get { return _isDying; } protected set { _isDying = value; } }
        public bool IsAttacking { get { return _isAttacking; } protected set { _isAttacking = value; } }
        #endregion

        #region Constructor
        public Entity(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed,
            Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> animations, Texture2D shadow, Camera camera)
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
            _animations = AnimatedSpriteHelper.GetAnimationData(GamePlayScreen.ContentRef, animations);
            _currentAnimationKey = EAnimationKey.Idle;
            _currentDirection = EDirection.Right;
            _camera = camera;
            _shadow = shadow;
        }
        #endregion

        #region Methods
        public virtual void Update(GameTime gameTime)
        {
            if (_isFlashing)
            {
                _flashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_flashTimer <= 0)
                {
                    _isFlashing = false;
                }
            }
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    for (int i = 0; i < _animations[_currentAnimationKey][_currentDirection].Keys.Count; i++)
                    {
                        EEquipmentSlot slot = _animations[_currentAnimationKey][_currentDirection].Keys.ElementAt(i);
                        _animations[_currentAnimationKey][_currentDirection][slot].Update(gameTime, _currentAnimationKey);
                    }
                }
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    Color drawColor = _isFlashing || _isDying ? GetFlashColor() : Color.White;
                    for (int i = 0; i < _animations[_currentAnimationKey][_currentDirection].Keys.Count; i++)
                    {
                        EEquipmentSlot slot = _animations[_currentAnimationKey][_currentDirection].Keys.ElementAt(i);
                        _animations[_currentAnimationKey][_currentDirection][slot].Draw(spriteBatch, _position, _currentDirection, _shadow, GetShadowPosition(), drawColor);
                    }                    
                }
            }
        }
        public virtual Circle GetHitbox(Vector2 desiredPosition)
        {
            Vector2 feetPosition = desiredPosition + new Vector2(8, 8); // on a 16px sprite, will be right in the middle
            return new Circle(feetPosition, 8f); // radius of 8 makes a circle 16px wide
        }
        public virtual Vector2 GetShadowPosition()
        {
            return new Vector2(_position.X, _position.Y + 2);
        }
        public virtual float GetDepthY()
        {
            Circle hitbox = GetHitbox(Position);
            return hitbox.Center.Y;
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
                        for (int i = 0; i < _animations[key][direction].Keys.Count; i++)
                        {
                            EEquipmentSlot slot = _animations[key][direction].Keys.ElementAt(i);
                            _animations[key][direction][slot].Reset();
                        }                        
                    }                    
                    _currentDirection = direction;                                     
                }                             
            }
        }
        public virtual void TakeDamage(int amount)
        {
            _healthPoints = MathHelper.Max(0, _healthPoints - amount);
            if (_healthPoints == 0)
            {
                _isDying = true;
                _currentAnimationKey = EAnimationKey.Death;
            }
            else
            {
                _isFlashing = true;
                _flashTimer = _flashDuration; // reset timer
            }
        }
        public void DealDamage(Entity target)
        {
            int damage = CalculateDamage();
            target.TakeDamage(damage);
        }
        public void Move(Vector2 direction)
        {
            if (_isDying)
            {
                return; // don't move when we are dying
            }

            Vector2 newPosition = _position + direction;
            Circle newHitBox = GetHitbox(newPosition);
            Vector2 totalDisplacement = Vector2.Zero;

            if (_currentMap != null)
            {
                if (_currentMap.TileMap.Obstacles != null)
                {
                    for (int i = 0; i < _currentMap.TileMap.Obstacles.Count; i++)
                    {
                        Rectangle obstacle = _currentMap.TileMap.Obstacles[i];
                        if (CollisionHelper.CircleIntersectsRectangle(newHitBox, obstacle, out Vector2 displacement))
                        {
                            totalDisplacement += displacement;
                        }
                    }
                }
                if (_currentMap.Entities.Count > 0)
                {
                    for (int i = 0; i < _currentMap.Entities.Count; i++)
                    {
                        Entity entity = _currentMap.Entities[i];
                        if (entity != this) // don't check against yourself
                        {
                            Circle enemyHitbox = entity.GetHitbox(entity.Position);
                            if (CollisionHelper.CircleIntersectsCircle(newHitBox, enemyHitbox, out Vector2 displacement))
                            {
                                totalDisplacement += displacement;
                            }
                        }
                    }
                }
            }
               
            // Apply the total displacement to the new position
            newPosition += totalDisplacement;
            CompleteMove(newPosition);
        }
        protected virtual int CalculateDamage()
        {
            // todo: customize calculation based on weapon, armor, stats, type, etc
            return 10;
        }
        public void LockToMap()
        {
            _position.X = MathHelper.Clamp(_position.X, 0, _currentMap.TileMap.MapWidth - _animations[_currentAnimationKey][_currentDirection][EEquipmentSlot.Feet].FrameWidth);
            _position.Y = MathHelper.Clamp(_position.Y, 0, _currentMap.TileMap.MapHeight - _animations[_currentAnimationKey][_currentDirection][EEquipmentSlot.Feet].FrameHeight);
        }
        public virtual Color GetFlashColor()
        {
            return Color.Lerp(Color.Red, Color.Black, _flashTimer / _flashDuration);
        }
        #endregion

        #region Private
        private void CompleteMove(Vector2 newPosition)
        {
            _position = newPosition;
            if (_animations.ContainsKey(_currentAnimationKey))
            {
                if (_animations[_currentAnimationKey].ContainsKey(_currentDirection))
                {
                    for (int i = 0; i < _animations[_currentAnimationKey][_currentDirection].Keys.Count; i++)
                    {
                        EEquipmentSlot slot = _animations[_currentAnimationKey][_currentDirection].Keys.ElementAt(i);
                        _animations[_currentAnimationKey][_currentDirection][slot].UpdatePosition(_position);
                    }                    
                }
            }
        }
        #endregion
    }
}
