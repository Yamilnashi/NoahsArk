using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Extensions;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public class StartMenuScreen : BaseGameState
    {
        #region Fields
        private PictureBox _backgroundImage;
        private PictureBox _arrowRightImage;
        private PictureBox _arrowLeftImage;
        private LinkLabel _newGame;
        private float _maxItemWidth = 0f;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public StartMenuScreen(Game game, GameStateManager manager) : base(game, manager)
        {
        }
        #endregion

        #region Methods
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = _gameRef.Content;
            _backgroundImage = new PictureBox(content.Load<Texture2D>("Assets/Backgrounds/title_background"), _gameRef.ScreenRectangle);
            _controlManager.Add(_backgroundImage);

            Texture2D arrowRightTexture = content.Load<Texture2D>("Assets/Menus/selector_right");
            Texture2D arrowLeftTexture = content.Load<Texture2D>("Assets/Menus/selector_left");
            _arrowRightImage = new PictureBox(
                    arrowRightTexture,
                    new Rectangle(0, 0, arrowRightTexture.Width, arrowRightTexture.Height)
                );
            _arrowLeftImage = new PictureBox(
                arrowLeftTexture,
                new Rectangle(0, 0, arrowLeftTexture.Width, arrowLeftTexture.Height)
                );

            _controlManager.Add(_arrowRightImage);
            _controlManager.Add(_arrowLeftImage);

            string[] menuOptions = new string[] { "New Game", "Load Game", "Settings", "Quit" };
            for (int i = 0; i < menuOptions.Length; i++)
            {
                LinkLabel option = CreateMenuOption(menuOptions[i]);
                _controlManager.Add(option);
            }

            _controlManager.NextControl();
            _controlManager.FocusChanged += new EventHandler(ControlManager_FocusChanged);            
            SetInitialControlPosition();
            ControlManager_FocusChanged(_newGame, null); // initial focus should be on first item

        }
        #endregion

        #region Private
        private LinkLabel CreateMenuOption(string optionText)
        {
            LinkLabel label = new LinkLabel(optionText, Color.Brown, Color.Black);
            label.Selected += new EventHandler(MenuItem_Selected);
            if (optionText.Equals("New Game", StringComparison.OrdinalIgnoreCase))
            {
                _newGame = label;
            }
            return label;
        }
        private void MenuItem_Selected(object sender, EventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            if (label.Text.Equals("New Game", StringComparison.OrdinalIgnoreCase))
            {
                Transition(EChangeType.Push, _gameRef.TitleScreen);
            }
            if (label.Text.Equals("Load Game", StringComparison.OrdinalIgnoreCase))
            {
                Transition(EChangeType.Push, _gameRef.TitleScreen);
            }
            if (label.Text.Equals("Settings", StringComparison.OrdinalIgnoreCase))
            {
                Transition(EChangeType.Push, _gameRef.TitleScreen);
            }
            if (label.Text.Equals("Quit", StringComparison.OrdinalIgnoreCase))
            {
                _gameRef.Exit();
            }
        }
        private void SetInitialControlPosition()
        {
            float bottomPosition = _gameRef.ScreenRectangle.Height - (_controlManager.Count * 75);
            var centerOfScreen = _gameRef.ScreenRectangle.GetCenter();
            for (int i = 0; i < _controlManager.Count; i++)
            {
                Control control = _controlManager[i];
                if (control is LinkLabel)
                {
                    if (control.Size.X > _maxItemWidth)
                    {
                        _maxItemWidth = control.Size.X;
                    }
                    control.Position = new Vector2(
                            centerOfScreen.X - control.Size.X / 2,
                            bottomPosition
                        );
                    bottomPosition += control.Size.Y + 5f; 
                }
            }
        }
        private void ControlManager_FocusChanged(object sender, EventArgs e)
        {
            if (sender is LinkLabel linkLabel)
            {
                // let's position the right arrow just to the left of the text
                _arrowRightImage.SetPosition(new Vector2(
                    linkLabel.Position.X - _arrowLeftImage.Size.X - 25f,
                    linkLabel.Position.Y + (linkLabel.Size.Y - _arrowLeftImage.Size.Y) / 2 - 5f
                ));

                // let's position the left arrow kust to the right of the text
                _arrowLeftImage.SetPosition(new Vector2(
                    linkLabel.Position.X + linkLabel.Size.X + 5f,
                    linkLabel.Position.Y + (linkLabel.Size.Y - _arrowRightImage.Size.Y) / 2 - 5f
                ));
            }
        }

        public override void Update(GameTime gameTime)
        {
            _controlManager.Update(gameTime, _playerIndex);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _gameRef.SpriteBatch.End();
        }
        #endregion

    }
}
