using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public class GamePlayScreen : BaseGameState
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public GamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _gameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _gameRef.SpriteBatch.End();
        }
        #endregion
    }
}
