using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Extensions;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public class TitleScreen : BaseGameState
    {
        #region Fields
        private Texture2D _backgroundImage;
        private LinkLabel _startLabel;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public TitleScreen(Game game, GameStateManager manager) : base(game, manager)
        {
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            ContentManager content = _gameRef.Content;
            _backgroundImage = content.Load<Texture2D>("Assets/Backgrounds/title_background");
            base.LoadContent();

            _startLabel = new LinkLabel("Press ENTER to begin", Color.Beige, Color.Black);
            Vector2 startLabelPosition = _gameRef.ScreenRectangle.GetBottomCenteredPosition(_startLabel.Size);
            _startLabel.Position = startLabelPosition;
            _startLabel.TabStop = true;
            _startLabel.HasFocus = true;
            _startLabel.Selected += new System.EventHandler(_startLabel_Selected);
            _controlManager.Add(_startLabel);
        }

        public override void Update(GameTime gameTime)
        {
            _controlManager.Update(gameTime, PlayerIndex.One);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            _gameRef.SpriteBatch.Draw(_backgroundImage, _gameRef.ScreenRectangle, Color.White);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _gameRef.SpriteBatch.End();
        }
        #endregion

        #region Private
        private void _startLabel_Selected(object sender, EventArgs e)
        {
            Transition(EChangeType.Push, _gameRef.StartMenuScreen);
        }
        #endregion

    }
}
