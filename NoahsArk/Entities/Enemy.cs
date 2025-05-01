using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Sprites;
using NoahsArk.Extensions;
using NoahsArk.Rendering;

namespace NoahsArk.Entities
{   
    public class Enemy : Entity
    {
        #region Fields
        private EEnemyType _enemyType;
        private Texture2D _rarityMarker;
        private static Dictionary<ERarity, Texture2D> _healthBarTexture;
        private static Dictionary<ERarity, Rectangle> _healthBarRectangle;
        private static Dictionary<ERarity, List<Rectangle>> _healthBarFrames;
        private IAIBehavior _behavior;
        private float _healthBarOpacity = 0.8f;
        private ERarity _rarityType;
        #endregion

        #region Properties
        public IAIBehavior Behavior { get; private set; }
        public static Dictionary<ERarity, Texture2D> HealthBarTexture {  get { return _healthBarTexture; } set { _healthBarTexture = value; } }
        public static Dictionary<ERarity, Rectangle> HealthBarRectangle { get { return _healthBarRectangle; } set { _healthBarRectangle = value; } }
        public static Dictionary<ERarity, List<Rectangle>> HealthBarFrames { get { return _healthBarFrames; } set { _healthBarFrames = value; } }
        public EEnemyType EnemyType { get { return _enemyType; } }
        public ERarity Rarity { get { return _rarityType; } }
        #endregion

        #region Constructor
        public Enemy(EEnemyType enemyType, int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed, ERarity rarity,
            Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> animations, Texture2D shadow, Texture2D rarityMarker, Camera camera,
            IAIBehavior behavior) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, shadow, camera)
        {
            _enemyType = enemyType;
            _behavior = behavior;
            _rarityType = rarity;
            _rarityMarker = rarityMarker;
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
                        CurrentMap.RemoveEnemy(this);
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
            if (_healthBarTexture != null &&
                    _healthBarFrames.Count > 0 &&
                    HealthPoints < MaxHealthPoints)
            {
                spriteBatch.Draw(_healthBarTexture[_rarityType], barPosition, _healthBarRectangle[_rarityType], Color.White * _healthBarOpacity);
                if (HealthPoints <= 0)
                {
                    spriteBatch.Draw(_healthBarTexture[_rarityType], barPosition, _healthBarFrames[_rarityType][6], Color.White * _healthBarOpacity);
                    return;
                }

                float healthPercent = (float)HealthPoints / MaxHealthPoints;
                int frameIndex = 0;
                if (healthPercent > 0.80f)
                {
                    frameIndex = 0;
                }
                else if (healthPercent >= 0.6f)
                {
                    frameIndex = 1;
                }
                else if (healthPercent >= 0.5f)
                {
                    frameIndex = 2;
                }
                else if (healthPercent >= 0.4f)
                {
                    frameIndex = 3;
                }
                else if (healthPercent >= 0.2f)
                {
                    frameIndex = 4;
                }
                else
                {
                    frameIndex = 5;
                }
                Rectangle sourceRect = _healthBarFrames[_rarityType][frameIndex];
                spriteBatch.Draw(_healthBarTexture[_rarityType], barPosition, sourceRect, Color.White * _healthBarOpacity);
            }
            int markerCount = _rarityType.GetRarityMarkerCount();
            if (markerCount > 0)
            {
                int markerWidth = 4;    // Width of each marker
                int totalWidth = (markerCount * markerWidth) + ((markerCount - 1)); // Total space needed
                float startX = rarityMarkerPosition.X - (totalWidth / 2f); // Center the markers

                for (int i = 0; i < markerCount; i++)
                {
                    Vector2 markerPosition = new Vector2(startX + (i * (markerWidth)), rarityMarkerPosition.Y);
                    spriteBatch.Draw(_rarityMarker, markerPosition, new Rectangle(0, 0, 4, 8), Color.White);
                }
            }

        }
        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
        }
        #endregion
        #region Private
        private Vector2 GetHealthBarPosition()
        {
            return GetHitbox(Position).Center + new Vector2(-25, -38); // on a 64x64 sprite
        }

        private Vector2 GetRarityMarkerPosition()
        {
            return GetHitbox(Position).Center + new Vector2(12, -28);
        }
        #endregion
    }
}
