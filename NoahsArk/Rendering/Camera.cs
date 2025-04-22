using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Levels;

namespace NoahsArk.Rendering
{
    public  class Camera
    {
        #region Fields
        private Vector2 _position;
        private float _speed;
        private float _zoom;
        private Rectangle _viewportRectangle;
        private ECameraMode _mode;
        #endregion
        #region Properties
        public Vector2 Position 
        {  
            get { return _position; } 
            set
            {
                _position = value;
                // snap to the pixel grid
                _position.X = (float)Math.Round(_position.X);
                _position.Y = (float)Math.Round(_position.Y);
            } 
        }
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                Speed = MathHelper.Clamp(Speed, 1f, 16f);
            }
        }
        public float Zoom { get { return _zoom; } }
        public ECameraMode CameraMode { get { return _mode; } }
        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateScale(_zoom) * Matrix.CreateTranslation(new Vector3(-Position, 0f));
            }
        }
        public Rectangle ViewportRectangle
        {
            get
            {
                return new Rectangle(                    
                    _viewportRectangle.X,
                    _viewportRectangle.Y,
                    _viewportRectangle.Width,
                    _viewportRectangle.Height);
            }
        }
        #endregion
        #region Constructor
        public Camera(Rectangle viewportRect)
        {
            _speed = 4f;
            _zoom = 3f;
            _viewportRectangle = viewportRect;
            _mode = ECameraMode.Follow;
        }
        #endregion
        #region Methods
        public void Update(GameTime gameTime, Map currentMap)
        {
            if (_mode == ECameraMode.Follow)
            {
                return;
            }

            Vector2 motion = Vector2.Zero;
            if (InputHandler.KeyDown(Keys.Left) ||
                InputHandler.ButtonDown(Buttons.RightThumbstickLeft, PlayerIndex.One))
                motion.X = -_speed;
            else if (InputHandler.KeyDown(Keys.Right) ||
                InputHandler.ButtonDown(Buttons.RightThumbstickRight, PlayerIndex.One))
                motion.X = _speed;

            if (InputHandler.KeyDown(Keys.Up) ||
                InputHandler.ButtonDown(Buttons.RightThumbstickUp, PlayerIndex.One))
                motion.Y = -_speed;
            else if (InputHandler.KeyDown(Keys.Down) ||
                InputHandler.ButtonDown(Buttons.RightThumbstickDown, PlayerIndex.One))
                motion.Y = _speed;

            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                Position += motion * _speed;
                LockCamera(currentMap);
            }
        }

        public void LockToPosition(Vector2 target, Map currentMap, Texture2D textureOffset = null)
        {
            float textureOffsetX = textureOffset != null
                ? textureOffset.Width / 2f
                : 0f;
            float textureOffsetY = textureOffset != null
                ? textureOffset.Height / 2f
                : 0f;

            float rawX = (target.X + textureOffsetX) * _zoom - _viewportRectangle.Width / 2f;
            float rawY = (target.Y + textureOffsetY) * _zoom - _viewportRectangle.Height / 2f;

            // snap to whole pixels
            _position.X = (float)Math.Round(rawX);
            _position.Y = (float)Math.Round(rawY);

            LockCamera(currentMap);
        }

        public void ZoomIn(Map currentMap)
        {
            _zoom += 1f;
            if (_zoom > 3f)
            {
                _zoom = 3f;
            }

            Vector2 newPosition = Position * _zoom;
            SnapToPosition(newPosition, currentMap);
        }

        public void ZoomOut(Map currentMap)
        {
            _zoom -= 1f;
            if (_zoom < 1f)
            {
                _zoom = 1f;
            }
            Vector2 newPosition = Position * _zoom;
            SnapToPosition(newPosition, currentMap);
        }

        public void SnapToPosition(Vector2 newPosition, Map currentMap)
        {
            float rawX = newPosition.X - _viewportRectangle.Width / 2;
            float rawY = newPosition.Y - _viewportRectangle.Height / 2;

            _position.X = (float)Math.Round(rawX);
            _position.Y = (float)Math.Round(rawY);  

            LockCamera(currentMap);
        }

        public void LockCamera(Map currentMap)
        {
            float maxX = currentMap.TileMap.MapWidth * _zoom - ViewportRectangle.Width;
            float maxY = currentMap.TileMap.MapHeight * _zoom - ViewportRectangle.Height;

            _position.X = MathHelper.Clamp(_position.X, 0, maxX);
            _position.Y = MathHelper.Clamp(_position.Y, 0, maxY);

            // snap after clamping
            _position.X = (float)Math.Round(_position.X);
            _position.Y = (float)Math.Round(_position.Y);
        }

        public void ToggleCameraMode()
        {
            if (_mode == ECameraMode.Follow)
            {
                _mode = ECameraMode.Free;
            } else if (_mode == ECameraMode.Free)
            {
                _mode = ECameraMode.Follow;
            }
        }

        public Point CameraPoint()
        {
            return Engine.VectorToCell(_position * (1 / _zoom));
        }

        public Point ViewPoint()
        {
            Vector2 viewportVector = new Vector2(
                    (_position.X + _viewportRectangle.Width) * (1 / _zoom),
                    (_position.Y + _viewportRectangle.Height) * (1 / _zoom)
                );
            return Engine.VectorToCell(viewportVector);
        }
        #endregion
    }
}
