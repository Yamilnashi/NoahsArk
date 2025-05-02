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
        private float _hitboxOffsetX;
        private float _hitboxOffsetY;
        private int _hitFrame;
        #endregion

        #region Properties
        public int FrameWidth { get { return _frameWidth; } }
        public int FrameHeight { get { return _frameHeight; } }
        public int CurrentFrame { get { return _currentFrame; } }
        public int TotalFrames { get { return _frames.Count; } }
        public int HitFrame { get { return _hitFrame; } }
        public bool IsHitFrame => CurrentFrame == HitFrame;
        #endregion

        #region Constructor
        public AnimatedSprite(Texture2D texture, int frameCount, int frameWidth, int frameHeight, 
            float frameDuration, float hitboxOffsetX, float hitboxOffsetY, int hitFrame)
        {
            _texture = texture;
            _frameDuration = frameDuration;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _hitboxOffsetX = hitboxOffsetX;
            _hitboxOffsetY = hitboxOffsetY; 
            _hitFrame = hitFrame;
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

        public void DrawShadow(SpriteBatch spriteBatch, Vector2 Position, Texture2D shadow, Vector2 shadowPosition)
        {
            spriteBatch.Draw(shadow, shadowPosition, 
                new Rectangle(0, 0, 16, 32), 
                Color.White * 0.3f, // some opacity
                0f, 
                new Vector2(0, 0), 
                1f, 
                SpriteEffects.None, 
                0f);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, EDirection direction, Color color)
        {   
            Vector2 topLeftPosition = position - new Vector2(_hitboxOffsetX, _hitboxOffsetY);
            SpriteEffects spriteEffects = direction == EDirection.Left
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
            spriteBatch.Draw(_texture, topLeftPosition, _frames[_currentFrame], color, 0f, new Vector2(0, 0), 1f, spriteEffects, 0f);
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
