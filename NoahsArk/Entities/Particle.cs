using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities
{
    public class Particle : IDrawableSortable
    {
        #region Fields
        private Vector2 _position;
        private Vector2 _velocity;
        private float _lifetime;
        private float _timeAlive;
        private Color _color;
        private float _size;
        private float _rotation;
        private float _rotationSpeed;
        private bool _isActive;
        private Texture2D _texture;
        private Random _random = new Random();
        #endregion

        #region Properties
        public bool IsActive { get { return _isActive; } }
        public Vector2 Position { get { return _position; } }
        #endregion

        #region Constructor
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float lifetime, Color color, float size, float rotationSpeed = 0f)
        {
            _texture = texture;
            _position = position;
            _velocity = velocity;
            _lifetime = lifetime;
            _color = color;
            _size = size;
            _rotation = 0f;
            _isActive = true;
            _rotationSpeed = rotationSpeed;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            if (!_isActive)
            {
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeAlive += deltaTime;
            _position += _velocity * deltaTime;
            _velocity += new Vector2(0, 50f) * deltaTime; // gravity, might need tweaking
            _rotation += _rotationSpeed * deltaTime;
            if (_timeAlive >= _lifetime)
            {
                _isActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isActive)
            {
                return;
            }

            // do some fade out
            float alpha = Math.Max(0, 1f - (_timeAlive / _lifetime * 2f));
            float currentSize = _size * alpha;
            float sizeJitter = currentSize * (1f + (float)_random.NextDouble() * 0.2f - 0.1f);
            Color drawColor = new Color(_color.R, _color.G, _color.B, alpha);
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
            spriteBatch.Draw(_texture, _position, null, drawColor, _rotation, origin, sizeJitter, SpriteEffects.None, 0f);
        }

        public float GetDepthY()
        {
            return _position.Y;
        }
        #endregion
    }
}
