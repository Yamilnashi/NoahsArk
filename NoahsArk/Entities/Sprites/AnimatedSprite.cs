using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;

namespace NoahsArk.Entities.Sprites
{
    public class AnimatedSprite
    {
        #region Fields
        private Texture2D _texture;
        private List<Rectangle> _frames;
        private int _frameWidth;
        private int _frameHeight;
        private int _currentFrame;
        private float _timer;
        private float _frameDuration;
        #endregion

        #region Properties
        public int FrameWidth { get { return _frameWidth; } }
        public int FrameHeight { get { return _frameHeight; } }
        #endregion

        #region Constructor
        public AnimatedSprite(Texture2D texture, int frameCount, int frameWidth, int frameHeight, float frameDuration)
        {
            _texture = texture;
            _frameDuration = frameDuration;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frames = new List<Rectangle>();
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle rectangle = new Rectangle(
                        i * frameWidth,
                        0,
                        frameWidth,
                        frameHeight
                    );
                _frames.Add(rectangle);
            }
            _currentFrame = 0;
            _timer = 0;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime, EAnimationKey currentAnimation)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float frameMultiplier = 3f;
            if (currentAnimation == EAnimationKey.Running)
            {
                frameMultiplier -= 1f;
            }
            if (_timer >= _frameDuration * frameMultiplier)
            {
                _timer = 0f;
                _currentFrame = (_currentFrame + 1) % _frames.Count;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, EDirection direction, Texture2D shadow, Vector2 shadowPosition)
        {            
            SpriteEffects spriteEffects = direction == EDirection.Left
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
            if (shadow != null)
            {
                spriteBatch.Draw(shadow, shadowPosition, new Rectangle(0, 0, 16, 32), Color.White * 0.3f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }            
            spriteBatch.Draw(_texture, position, _frames[_currentFrame], Color.White, 0f, new Vector2(0, 0), 1f, spriteEffects, 0f);
        }
        public void UpdatePosition(Vector2 newPosition)
        {
            // todo:
        }
        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0;
        }        
        #endregion
    }
}
