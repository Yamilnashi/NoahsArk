using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;

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
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations,
            IAIBehavior behavior) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations)
        {
            _behavior = behavior;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            _behavior.Update(this, gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}
