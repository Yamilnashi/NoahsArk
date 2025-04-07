using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Controls
{
    public class PictureBox : Control
    {
        #region Fields
        private Texture2D _image;
        private Rectangle _sourceRect;
        private Rectangle _destinationRect;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public PictureBox(Texture2D image, Rectangle destination)
        {
            _image = image;
            _destinationRect = destination;
            _sourceRect = new Rectangle(0, 0, _image.Width, _image.Height);
            Color = Color.White;
        }
        public PictureBox(Texture2D image, Rectangle destination, Rectangle source)
        {
            _image = image;
            _destinationRect = destination;
            _sourceRect = source;
            Color = Color.White;
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
            spriteBatch.Draw(_image, _destinationRect, _sourceRect, Color);
        }
        public void SetPosition(Vector2 newPosition)
        {
            _destinationRect = new Rectangle(
                    (int)newPosition.X,
                    (int)newPosition.Y,
                    _sourceRect.Width,
                    _sourceRect.Height
                );
        }
        #endregion
    }
}
