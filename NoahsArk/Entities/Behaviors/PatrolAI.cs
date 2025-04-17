using System;
using Microsoft.Xna.Framework;

namespace NoahsArk.Entities.Behaviors
{
    public class PatrolAI : IAIBehavior
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        #endregion

        #region Methods
        public void Update(Enemy enemy, GameTime gameTime)
        {
            enemy.SetAnimation(Sprites.EAnimationKey.Walking, Controls.EDirection.Right);
        }
        #endregion
    }
}
