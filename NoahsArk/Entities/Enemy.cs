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
        private IAIBehavior _behavior;
        #endregion

        #region Properties
        public IAIBehavior Behavior { get; private set; }
        #endregion

        #region Constructor
        public Enemy(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed, 
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations, Texture2D shadow, Camera camera,
            IAIBehavior behavior) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, shadow, camera)
        {
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
        public override void Update(GameTime gameTime)
        {       
            _behavior.Update(this, gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}
