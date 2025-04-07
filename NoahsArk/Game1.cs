using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Managers;
using NoahsArk.States;

namespace NoahsArk
{
    public class Game1 : Game
    {
        #region Fields

        #region Frame Rates
        private float _fps;
        private float _updateInterval = 1.0f;
        private float _timeSinceLastUpdate = 0.0f;
        private float _frameCount = 0;
        #endregion

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private const int _screenWidth = 1280;
        private const int _screenHeight = 720;
        private readonly Rectangle _screenRectangle = new Rectangle(0, 0, _screenWidth, _screenHeight);

        private TitleScreen _titleScreen;
        private StartMenuScreen _startMenuScreen;
        #endregion

        #region Properties
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        public Rectangle ScreenRectangle { get { return _screenRectangle; } }   
        public TitleScreen TitleScreen { get { return _titleScreen; } }
        public StartMenuScreen StartMenuScreen {  get { return _startMenuScreen; } }
        #endregion

        #region Constructor
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            Components.Add(new InputHandler(this));

            // add game state manager, texture manager, all screens, then change state to new state
            _gameStateManager = new GameStateManager(this);
            Components.Add(_gameStateManager);

            _titleScreen = new TitleScreen(this, _gameStateManager);
            _startMenuScreen = new StartMenuScreen(this, _gameStateManager);

            _gameStateManager.ChangeState(TitleScreen);
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
        }
        #endregion

        #region Methods
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // read data from data manager
            // load fonts
            // add texture from texturemanager
            // LoadEntityData(); // items, weapons, equipment
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }               

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            SetFrameCounter(gameTime);
        }
        #endregion

        #region Private
        private void SetFrameCounter(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCount++;
            _timeSinceLastUpdate += elapsed;

            if (_timeSinceLastUpdate > _updateInterval)
            {
                _fps = _frameCount / _timeSinceLastUpdate;
                this.Window.Title = $"FPS: {_fps}";

                _frameCount = 0;
                _timeSinceLastUpdate -= _updateInterval;
            }
        }

        private void LoadFonts()
        {

        }

        private void LoadEntityData()
        {

        }
        #endregion
    }
}
