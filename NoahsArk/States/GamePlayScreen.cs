using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Levels;
using NoahsArk.Levels.Maps;
using NoahsArk.Managers;
using NoahsArk.Rendering;

namespace NoahsArk.States
{
    public class GamePlayScreen : BaseGameState
    {
        #region Fields
        private Engine _engine = new Engine(16, 16);
        private World _world;
        private Camera _camera;
        #endregion

        #region Properties
        public Engine Engine { get { return _engine; } }
        #endregion

        #region Constructor
        public GamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            _camera = new Camera(_gameRef.ScreenRectangle);
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            // CreatePlayer();
            // LoadMusic();
            LoadWorld();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _camera.Update(gameTime);
            _world.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                _camera.Transformation);
            base.Draw(gameTime);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _world.Draw(_gameRef.SpriteBatch, gameTime, _camera);
            _gameRef.SpriteBatch.End();
        }
        #endregion

        #region Private
        private void LoadWorld()
        {
            _world = new World(_gameRef);
            Game.Components.Add(_world);
        }
        #endregion
    }
}
