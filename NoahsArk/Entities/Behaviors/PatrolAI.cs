using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NoahsArk.Controls;

namespace NoahsArk.Entities.Behaviors
{
    public class PatrolAI : IAIBehavior
    {
        #region Fields
        private Vector2 _currentDirection;
        private float _timeInDirection;
        private float _timeToChangeDirection;
        private float _minTimeInDirection = 1f;
        private float _maxTimeInDirection = 3f;
        private Random _random;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public PatrolAI()
        {
            _random = new Random();
            _currentDirection = GetRandomDirection();
            _timeToChangeDirection = GetRandomTime();
        }
        #endregion

        #region Methods
        public void Update(Enemy enemy, GameTime gameTime)
        {
            _timeInDirection += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeInDirection >= _timeToChangeDirection)
            {
                _timeInDirection = 0f;
                _currentDirection = GetRandomDirection();
                _timeToChangeDirection = GetRandomTime();
            }

            EDirection animationDirection = enemy.CurrentDirection;
            if (_currentDirection.X > 0)
            {
                animationDirection = EDirection.Right;
            }
            else if (_currentDirection.X < 0)
            {
                animationDirection = EDirection.Left;
            }

            enemy.SetAnimation(Sprites.EAnimationKey.Walking, animationDirection);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 displacement = _currentDirection * enemy.Speed * deltaTime;
            enemy.Move(displacement);
            enemy.LockToMap();
        }
        #endregion

        #region Private
        private Vector2 GetRandomDirection()
        {
            double angle = _random.NextDouble() * MathHelper.TwoPi;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        private float GetRandomTime()
        {
            return (float)(_random.NextDouble() * (_maxTimeInDirection - _minTimeInDirection) + _minTimeInDirection);
        }
        #endregion
    }
}
