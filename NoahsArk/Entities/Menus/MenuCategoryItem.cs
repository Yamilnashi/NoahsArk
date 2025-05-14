using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities.Menus
{
    public class MenuCategoryItem
    {
        #region Fields
        private EMenuCategoryType _menuCategoryType;
        private Vector2 _position;
        private bool _isActive;
        private Texture2D _containerTexture;
        private Texture2D _icon;
        #endregion

        #region Properties
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)_position.X,
                    (int)_position.Y,
                    _containerTexture.Width,
                    _containerTexture.Height);
            }
        }
        public bool IsActive {  get { return _isActive; } set { _isActive = value; } } 
        #endregion

        #region Constructor
        public MenuCategoryItem(EMenuCategoryType menuCategoryType, Vector2 position, bool isActive,
            Texture2D containerTexture, Texture2D icon)
        {
            _menuCategoryType = menuCategoryType;
            _position = position;
            _isActive = isActive;
            _icon = icon;
            _containerTexture = containerTexture;
        }
        #endregion

        #region Methods
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color drawColor = _isActive ? Color.White : Color.White * 0.5f;
            spriteBatch.Draw(_containerTexture, _position, drawColor);
            spriteBatch.Draw(_icon, _position, drawColor);
        }
        #endregion
    }
}
