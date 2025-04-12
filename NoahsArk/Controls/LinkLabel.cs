using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Managers;

namespace NoahsArk.Controls
{
    internal class LinkLabel : Control
    {
        #region Properties
        #endregion

        #region Constructor
        public LinkLabel(string fontName, int fontSize, string text, Color textColor, Color textColorOnFocus)
        {
            TabStop = true;
            HasFocus = false;
            Position = Vector2.Zero;
            FocusColor = textColorOnFocus;
            Color = textColor;
            Text = text;
            SpriteFont = ControlManager.SpriteFont(fontName, fontSize);
            Size = SpriteFont.MeasureString(text);
        }
        #endregion

        #region Methods
        public override void HandleInput(PlayerIndex playerIndex)
        {
            if (!HasFocus)
            {
                return;
            }

            if (InputHandler.KeyReleased(Keys.Enter) ||
                InputHandler.ButtonReleased(Buttons.A, playerIndex))
            {
                base.OnSelected(null);
            }

            if (InputHandler.CheckMouseReleased(EMouseButton.Left))
            {
                if (GetBounds().Contains(InputHandler.MouseAsPoint))
                {
                    base.OnSelected(null);
                }
            }
        }
        public override void Update(GameTime gameTime)
        {            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Color textColor = HasFocus ? FocusColor : Color;
            spriteBatch.DrawString(SpriteFont, Text, Position, textColor);
        }
        #endregion
    }
}
