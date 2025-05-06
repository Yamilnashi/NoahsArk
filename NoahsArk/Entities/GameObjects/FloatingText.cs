using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities.GameObjects
{
    public class FloatingText
    {
        #region Fields
        private string _text;
        private Vector2 _position;
        private Vector2 _velocity;
        private Color _color;
        private float _lifetime;
        private float _intitialLifetime;
        private SpriteFont _font;
        #endregion

        #region Properties
        public float Lifetime { get { return _lifetime; } }
        #endregion

        #region Constructor
        public FloatingText(string text, Vector2 position, Vector2 velocity, Color color, 
            float lifetime, SpriteFont font)
        {
            _text = text;
            _position = position;
            _velocity = velocity;
            _color = color;
            _lifetime = lifetime;
            _intitialLifetime = lifetime;
            _font = font;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position += _velocity * deltaTime;
            _lifetime -= deltaTime;
            float alpha = MathHelper.Clamp(_lifetime / _intitialLifetime, 0, 1);
            _color = new Color(_color.R, _color.G, _color.B, (byte)(alpha * 255));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_lifetime > 0 )
            {
                spriteBatch.DrawString(_font, _text, _position, _color);
            }
        }
        #endregion
    }
}
