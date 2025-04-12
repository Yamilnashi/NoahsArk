using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Managers;

namespace NoahsArk.Controls
{
    public class Label : Control
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Label(string fontName, int fontSize, string text)
        {
            SpriteFont = ControlManager.SpriteFont(fontName, fontSize);
            _text = text;
            Size = SpriteFont.MeasureString(_text);
        }
        #endregion

        #region Methods
        public override void HandleInput(PlayerIndex playerIndex)
        {
        }
        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (SpriteFont != null &&
                !string.IsNullOrEmpty(_text))
            {
                spriteBatch.DrawString(SpriteFont, _text, _position, _color);
            }
        }
        #endregion
    }
}
