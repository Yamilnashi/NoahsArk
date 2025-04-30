using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Sprites;
using NoahsArk.Rendering;

namespace NoahsArk.Entities
{   
    public class Enemy : Entity
    {
        #region Fields
        private EEnemyType _enemyType;
        private static Texture2D _healthBarTexture;
        private static Rectangle _healthBarRectangle;
        private static List<Rectangle> _healthBarFrames;
        private IAIBehavior _behavior;
        #endregion

        #region Properties
        public IAIBehavior Behavior { get; private set; }
        public static Texture2D HealthBarTexture {  get { return _healthBarTexture; } set { _healthBarTexture = value; } }
        public static Rectangle HealthBarRectangle { get { return _healthBarRectangle; } set { _healthBarRectangle = value; } } 
        public static List<Rectangle> HealthBarFrames {  get { return _healthBarFrames; } set { _healthBarFrames = value; } }
        #endregion

        #region Constructor
        public Enemy(EEnemyType enemyType, int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed, 
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> animations, Texture2D shadow, Camera camera,
            IAIBehavior behavior) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, shadow, camera)
        {
            _enemyType = enemyType;
            _behavior = behavior;
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
            if (HealthPoints <= 0)
            {
                CurrentMap.RemoveEnemy(_enemyType, this);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw the sprite
            base.Draw(spriteBatch);

            if (_healthBarTexture != null &&
                _healthBarFrames.Count > 0 &&
                HealthPoints < MaxHealthPoints)
            {
                float healthPercent = (float)HealthPoints / MaxHealthPoints;

                int frameIndex = 0;
                if (healthPercent > 0.80f)
                {
                    frameIndex = 0;
                } else if (healthPercent >= 0.6f)
                {
                    frameIndex = 1;
                } else if (healthPercent >= 0.5f)
                {
                    frameIndex = 2;
                } else if (healthPercent >= 0.4f)
                {
                    frameIndex = 3;
                } else if (healthPercent >= 0.2f)
                {
                    frameIndex = 4;
                }

                Rectangle sourceRect = _healthBarFrames[frameIndex];
                Vector2 barPosition = GetHitbox(Position).Center + new Vector2(-25, -38);
                spriteBatch.Draw(_healthBarTexture, barPosition, sourceRect, Color.White * 0.8f);
            }
        }
        #endregion
    }
}
