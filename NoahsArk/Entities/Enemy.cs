using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities.Enemies;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Items;
using NoahsArk.Entities.Items.Weapons;
using NoahsArk.Entities.Sprites;
using NoahsArk.Extensions;
using NoahsArk.Rendering;
using NoahsArk.States;

namespace NoahsArk.Entities
{   
    public class Enemy : Entity
    {
        #region Fields
        private float _experienceRewardPoints;
        private EEnemyType _enemyType;
        private Texture2D _rarityMarker;
        private static Dictionary<ERarity, Texture2D> _healthBarTexture;
        private static Dictionary<ERarity, Rectangle> _healthBarRectangle;
        private IAIBehavior _behavior;
        private float _healthBarOpacity = 0.8f;
        private ERarity _rarityType;
        private Rectangle _healthBarFill;
        private Texture2D _healthBarFillTexture;
        #endregion

        #region Properties
        public IAIBehavior Behavior { get; private set; }
        public static Dictionary<ERarity, Texture2D> HealthBarTexture {  get { return _healthBarTexture; } set { _healthBarTexture = value; } }
        public static Dictionary<ERarity, Rectangle> HealthBarRectangle { get { return _healthBarRectangle; } set { _healthBarRectangle = value; } }
        public EEnemyType EnemyType { get { return _enemyType; } }
        public ERarity Rarity { get { return _rarityType; } }
        #endregion

        #region Constructor
        public Enemy(EEnemyType enemyType, float maxHealthPoints, float maxManaPoints, float experienceRewardPoints, Vector2 initialPosition, float speed, 
            ERarity rarity, Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> animations, Texture2D shadow, 
            Texture2D rarityMarker, Camera camera, IAIBehavior behavior) : base(maxHealthPoints, maxManaPoints, experienceRewardPoints, initialPosition, speed, animations, shadow, camera)
        {
            _experienceRewardPoints = experienceRewardPoints;
            _enemyType = enemyType;
            _behavior = behavior;
            _rarityType = rarity;
            _rarityMarker = rarityMarker;
            _healthBarFillTexture = new Texture2D(GamePlayScreen.GraphicsDeviceRef, 1, 1);
            _healthBarFillTexture.SetData(new[] { Color.Red });
            UpdateHealthBar();
        }
        #endregion

        #region Methods
        public override Circle GetHitbox(Vector2 desiredPosition)
        {
            if (CurrentAnimation == EAnimationKey.Idle)
            {
                Vector2 feetPosition = desiredPosition + new Vector2(16, 24); // on a 16px sprite, will be right in the middle
                return new Circle(feetPosition, 8f); // radius of 8 makes a circle 16px wide
            } else
            {
                Vector2 feetPosition = desiredPosition + new Vector2(32, 56); // on a 64px sprite, will be right in the middle
                return new Circle(feetPosition, 8f); // radius of 8 makes a circle 16px wide
            }            
        }
        public override Vector2 GetShadowPosition()
        {
            if (CurrentAnimation == EAnimationKey.Idle)
            {
                return Position + new Vector2(16, 24); // on a 16px sprite, will be right in the middle
            }
            else
            {
                return Position + new Vector2(24, 34); // on a 64px sprite, will be right in the middle
            }
        }
        public override void Update(GameTime gameTime)
        {       
            _behavior.Update(this, gameTime);
            base.Update(gameTime);
            if (HealthPoints <= 0 && CurrentAnimation == EAnimationKey.Death)
            {
                for (int i = 0; i < Animations[CurrentAnimation][CurrentDirection].Keys.Count; i++)
                {
                    EEquipmentSlot slot = Animations[CurrentAnimation][CurrentDirection].Keys.ElementAt(i);
                    int currentFrame = Animations[CurrentAnimation][CurrentDirection][slot].CurrentFrame;
                    int totalFrames = Animations[CurrentAnimation][CurrentDirection][slot].TotalFrames - 1;
                    if (currentFrame == totalFrames)
                    {                        
                        IsDying = false;
                        if (CurrentMap != null)
                        {
                            GenerateLoot();
                            CurrentMap.RemoveEnemy(this);
                        }                        
                    }
                }                         
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw the sprite
            base.Draw(spriteBatch);
            Vector2 barPosition = GetHealthBarPosition();
            Vector2 rarityMarkerPosition = GetRarityMarkerPosition();
            UpdateHealthBar();
            if (HealthPoints < MaxHealthPoints &&
                _healthBarTexture != null)
            {                
                if (HealthPoints > 0)
                {
                    spriteBatch.Draw(_healthBarFillTexture, _healthBarFill, Color.Red * _healthBarOpacity);
                }
                spriteBatch.Draw(_healthBarTexture[_rarityType], barPosition, _healthBarRectangle[_rarityType], Color.White * _healthBarOpacity);
            }

            int markerCount = _rarityType.GetRarityMarkerCount();
            if (markerCount > 0)
            {
                int markerWidth = 4;
                int totalWidth = (markerCount * markerWidth) + (markerCount - 1); // total space needed
                float startX = rarityMarkerPosition.X - (totalWidth / 2f); // center the markers
                
                for (int i = 0; i < markerCount; i++)
                {
                    Vector2 markerPosition = new Vector2(startX + (i * (markerWidth)), rarityMarkerPosition.Y);
                    spriteBatch.Draw(_rarityMarker, markerPosition, new Rectangle(0, 0, 4, 8), Color.White);
                }
            }
        }
        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            UpdateHealthBar();
        }
        #endregion
        #region Private
        private void GenerateLoot()
        {
            if (GamePlayScreen.EnemyEntityDict.TryGetValue(_enemyType, out Dictionary<ERarity, EnemyEntity> enemyDict) &&
                enemyDict.TryGetValue(_rarityType, out EnemyEntity entity) &&
                entity.LootTable != null)
            {
                Random random = new Random();
                for (int i = 0; i < entity.LootTable.Count; i++)
                {
                    LootDrop drop = entity.LootTable[i];
                    if (random.NextDouble() < drop.DropChance)
                    {
                        Item item = null;
                        int quantity = random.Next(drop.MinQuantity, drop.MaxQuantity + 1);
                        if (drop.ItemType == EItemType.Gold)
                        {
                            item = new Gold() { Amount = quantity };
                        }
                        else if (drop.ItemType == EItemType.Weapon && 
                            drop.WeaponType != null &&
                            drop.MaterialType != null) {
                            if (GamePlayScreen.WeaponObjectDict.TryGetValue((drop.WeaponType.Value, drop.MaterialType.Value), out WeaponObject weapon))
                            {
                                item = weapon;
                                quantity = 1; // cannot stack weapons
                            }
                        }

                        if (item != null)
                        {
                            Vector2 scatterOffset = new Vector2(
                                (float)(random.NextDouble() * 64 - 16),
                                (float)(random.NextDouble() * 64 - 16)
                            );
                            DroppedItem droppedItem = new DroppedItem(item, quantity, Position + scatterOffset, GetTextureForItem(item));
                            CurrentMap.AddDroppedItem(droppedItem);
                        }
                    }
                }
            }
        }
        private Texture2D GetTextureForItem(Item item)
        {
            if (item is Gold)
            {
                return GamePlayScreen.GoldSpriteSheet;
            }
            else if (item is WeaponObject weapon)
            {
                return weapon.GroundTexture;
            }
            return null;
        }
        private void UpdateHealthBar()
        {
            Vector2 barPosition = GetHealthBarPosition();
            float healthPercent = (float)HealthPoints / MaxHealthPoints;
            _healthBarFill = GetHealthBarFillbyRarityType(barPosition, healthPercent);
        }
        private Vector2 GetHealthBarPosition()
        {
            return GetHitbox(Position).Center + new Vector2(-25, -38); // on a 64x64 sprite
        }

        private Vector2 GetRarityMarkerPosition()
        {
            return GetHitbox(Position).Center + new Vector2(12, -28);
        }
        private Rectangle GetHealthBarFillbyRarityType(Vector2 barPosition, float healthPercent)
        {
            int width = 30;
            int height = 4;
            int barPositionX = (int)barPosition.X;
            int barPositionY = (int)barPosition.Y;
            switch (_rarityType) {
                case ERarity.Normal:
                case ERarity.Magic:
                case ERarity.Rare:
                case ERarity.Epic:
                    barPositionX += 10;
                    barPositionY += 6;
                    break;
                case ERarity.Legendary:
                    width = 42;
                    height = 5;
                    barPositionY += 7;
                    barPositionX += 4;
                        break;
            }
            int fillWidth = (int)(width * healthPercent);            
            return new Rectangle(barPositionX, barPositionY, fillWidth, height);
        }
        #endregion
    }
}
