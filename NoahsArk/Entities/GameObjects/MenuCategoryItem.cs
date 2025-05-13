using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities.GameObjects
{
    public class MenuCategoryItem
    {
        #region Fields
        private EMenuCategoryType _menuCategoryType;
        private Vector2 _position;
        private bool _isActive;
        private Dictionary<bool, Texture2D> _activeTextureDict;
        private Texture2D _activeIcon;
        private Texture2D _inactiveIcon;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public MenuCategoryItem(EMenuCategoryType menuCategoryType, Vector2 position, bool isActive,
            Texture2D activeTexture, Texture2D inactiveTexture, Texture2D activeIcon, Texture2D inactiveIcon)
        {
            _menuCategoryType = menuCategoryType;
            _position = position;
            _isActive = isActive;
            _activeIcon = activeIcon;
            _inactiveIcon = inactiveIcon;
            _activeTextureDict = new Dictionary<bool, Texture2D>()
            {
                { true, activeTexture },
                { false, inactiveTexture }
            };
        }
        #endregion

        #region Methods
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float textureOpacity = _isActive ? 1.0f : 0.5f;
            Vector2 iconPosition = _isActive ? _position : new Vector2(_position.X, _position.Y + 15); // slightly lower
            Texture2D icon = _isActive ? _activeIcon : _inactiveIcon;
            Vector2 containerPosition = _isActive ? _position : new Vector2(_position.X, _position.Y - 2); // slightly higher
            spriteBatch.Draw(_activeTextureDict[_isActive], containerPosition, Color.White * textureOpacity);
            spriteBatch.Draw(icon, iconPosition, Color.White * textureOpacity);
        }
        #endregion
    }
}
