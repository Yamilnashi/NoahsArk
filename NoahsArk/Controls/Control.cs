using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Managers;

namespace NoahsArk.Controls
{
    public abstract class Control
    {
        #region Fields
        protected string _name;
        protected string _text;
        protected Vector2 _size;
        protected Vector2 _position;
        protected object _value;
        protected bool _hasFocus;
        protected bool _isEnabled;
        protected bool _isVisible;
        protected bool _tabStop;
        protected SpriteFont _spriteFont;
        protected Color _color;
        protected Color _focusColor;
        protected string _type;
        protected Dictionary<string, object> _propertiesDict;
        #endregion

        #region Events
        public event EventHandler Selected;
        #endregion

        #region Properties
        public bool IsEnabled { get { return _isEnabled; } }
        public bool HasFocus {  get { return _hasFocus; } set { _hasFocus = value; } }
        public bool IsVisible { get { return _isVisible; } }
        public bool TabStop { get { return _tabStop; } set { _tabStop = value; } }
        public Vector2 Position { get { return _position; } set { _position = value; } }
        public Vector2 Size { get { return _size; } set { _size = value; } }
        public Color Color { get { return _color; } set { _color = value; } }
        public Color FocusColor { get { return _focusColor; } set { _focusColor = value; } }
        public SpriteFont SpriteFont { get { return _spriteFont; } set { _spriteFont = value; } }
        public string Text { get { return _text; } set { _text = value; } }
        public Dictionary<string, object> PropertiesDict {  get { return _propertiesDict; } set { _propertiesDict = value; } }
        #endregion

        #region Constructor
        public Control()
        {
            _color = Color.White;
            _focusColor = Color.Red;
            _isEnabled = true;
            _isVisible = true;
            _spriteFont = ControlManager.SpriteFont("Silver", 28);
            _propertiesDict = new Dictionary<string, object>();
        }
        #endregion

        #region Methods
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void HandleInput(PlayerIndex playerIndex);
        protected virtual void OnSelected(EventArgs e)
        {
            if (Selected != null)
            {
                Selected(this, e);
            }
        }
        public virtual Rectangle GetBounds()
        {
            return new Rectangle(
                    (int)_position.X,
                    (int)_position.Y,
                    (int)_size.X,
                    (int)_size.Y
                );
        }
        #endregion
    }
}
