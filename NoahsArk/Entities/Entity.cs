using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Items;
using NoahsArk.Entities.Items.Weapons;
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
        private float _healthPoints;
        private float _maxHealthPoints;
        private float _manaPoints;
        private float _maxManaPoints;
        private float _experiencePoints;
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
        public float HealthPoints {  get { return _healthPoints; } protected set { _healthPoints = value; } }
        public float MaxHealthPoints { get { return _maxHealthPoints; } protected set { _maxHealthPoints = value; } }
        public float ManaPoints { get { return _manaPoints; } protected set { _manaPoints = value; } }
        public float MaxManaPoints { get { return _maxManaPoints; } protected set { _maxManaPoints = value; } }
        public float ExperiencePoints { get { return _experiencePoints; } protected set { _experiencePoints = value; } }
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
        public Entity(int maxHealthPoints, int maxManaPoints, Vector2 initialTopLeftPosition, float speed,
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
            _position = initialTopLeftPosition;
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
                    _animations[_currentAnimationKey][_currentDirection][EEquipmentSlot.Feet].DrawShadow(spriteBatch, _position, _shadow, GetShadowPosition());
                    Color drawColor = _isFlashing || _isDying ? GetFlashColor() : Color.White;
                    for (int i = 0; i < _animations[_currentAnimationKey][_currentDirection].Keys.Count; i++)
                    {
                        EEquipmentSlot slot = _animations[_currentAnimationKey][_currentDirection].Keys.ElementAt(i);
                        _animations[_currentAnimationKey][_currentDirection][slot].Draw(spriteBatch, _position, _currentDirection, drawColor);
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
        public virtual void TakeDamage(float amount)
        {
            _healthPoints = MathHelper.Max(0f, _healthPoints - amount);
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
            float damage = CalculateDamage(out bool isCrit);
            target.TakeDamage(damage);
            Vector2 textPosition = target.GetHitbox(target.Position).Center + new Vector2(0, -30);
            Color color = isCrit
                ? Color.Yellow
                : Color.White;
            float lifetime = isCrit
                ? 2.0f
                : 1.0f;
            int size = isCrit
                ? 12
                : 10;
            CurrentMap.AddFloatingText(damage.ToString(), textPosition, color, lifetime, size);
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
        protected virtual float CalculateDamage(out bool isCrit)
        {
            if (_equippedItems.TryGetValue(EEquipmentSlot.MainHand, out Item item) && 
                item != null)
            {
                if (item is WeaponObject weapon)
                {
                    return weapon.CalculateDamage(out isCrit);   
                }
            }
            isCrit = false;
            return 0;
        }
        public void LockToMap()
        {
            Circle currentPositionHitbox = GetHitbox(_position);
            _position.X = MathHelper.Clamp(_position.X, 0, _currentMap.TileMap.MapWidth - currentPositionHitbox.Radius);
            _position.Y = MathHelper.Clamp(_position.Y, 0, _currentMap.TileMap.MapHeight - currentPositionHitbox.Radius * 3);
        }
        public virtual Color GetFlashColor()
        {
            return Color.Lerp(Color.Red, Color.Black, _flashTimer / _flashDuration);
        }
        public void EquipWeapon(EWeaponType weaponType, EMaterialType materialType)
        {
            if (GamePlayScreen.WeaponObjectDict.TryGetValue((weaponType, materialType), out WeaponObject weapon) && 
                weapon != null)
            {
                _equippedItems[EEquipmentSlot.MainHand] = weapon;
                UpdateWeaponAnimations(weapon);
            }
        }
        public virtual void UpdateWeaponAnimations(WeaponObject weapon)
        {
            for (int i = 0; i < weapon.Animations.Keys.Count; i++)
            {
                EAnimationKey animationKey = weapon.Animations.Keys.ElementAt(i);
                for (int j = 0; j < weapon.Animations[animationKey].Keys.Count; j++)
                {
                    EDirection direction = weapon.Animations[animationKey].Keys.ElementAt(j);
                    string filePath = weapon.Animations[animationKey][direction];
                    string formattedFilePath = DirectoryPathHelper.GetFormattedFilePath(filePath);
                    Texture2D texture = GamePlayScreen.ContentRef.Load<Texture2D>(formattedFilePath);
                    _animations[animationKey][direction][EEquipmentSlot.MainHand].SetTexture(texture);
                }
            }
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
