using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Extensions;
using NoahsArk.Levels;
using NoahsArk.Managers;
using NoahsArk.Rendering;

namespace NoahsArk.States
{
    public class PauseMenuScreen : BaseGameState
    {
        #region Fields
        private List<EMapCode> _mapCodes;
        private PictureBox _backgroundImage;
        private PictureBox _arrowRightImage;
        private PictureBox _arrowLeftImage;
        private int _selectedIndex = 0;
        private float _maxItemWidth = 0f;
        private Player _player;
        private Camera _camera;
        private EMapCode _currentMap;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public PauseMenuScreen(Game game, GameStateManager manager, Player player, Camera camera) : base(game, manager)
        {
            _mapCodes = Enum.GetValues(typeof(EMapCode))
                .Cast<EMapCode>()
                .ToList();
            _player = player;
            _camera = camera;
            GamePlayScreen screen = (GamePlayScreen)_gameStateManager.CurrentState;
            _currentMap = screen.World.CurrentMapCode;
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = _gameRef.Content;
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

            string[] menuOptions = _mapCodes.Select(x => x.ToString()).ToArray();
            for (int i = 0;  i < menuOptions.Length; i++)
            {
                LinkLabel option = CreateMenuOption(menuOptions[i]);
                _controlManager.Add(option);
            }
            
            _controlManager.FocusChanged += new EventHandler(ControlManager_FocusChanged);
            SetInitialControlPosition();
            ControlManager_FocusChanged(_controlManager
                .Where(x => !string.IsNullOrEmpty(x.Text))
                .FirstOrDefault(x => x.Text
                                    .Equals(_currentMap.ToString(), StringComparison.OrdinalIgnoreCase)),
                                    null);
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

        #region Private
        private LinkLabel CreateMenuOption(string optionText)
        {
            LinkLabel label = new LinkLabel("Silver", 28, optionText, Color.Brown, Color.Yellow);
            label.Selected += new EventHandler(MenuItem_Selected);
            label.HasFocus = _currentMap.ToString().Equals(optionText, StringComparison.OrdinalIgnoreCase);
            return label;
        }
        private void MenuItem_Selected(object sender, EventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            EMapCode mapCode = (EMapCode)Enum.Parse(typeof(EMapCode), label.Text, true);
            _gameStateManager.PopState();
            if (_gameStateManager.CurrentState is GamePlayScreen gamePlayScreen)
            {
                gamePlayScreen.World.CurrentMap.RemovePlayer(_player);
                gamePlayScreen.World.SetCurrentMap(mapCode);
                gamePlayScreen.World.CurrentMap.AddPlayer(_player);
                _camera.LockToPosition(_player.Position, gamePlayScreen.World.CurrentMap);
            }
        }
        private void ControlManager_FocusChanged(object sender, EventArgs e)
        {
            if (sender is LinkLabel linkLabel)
            {
                Vector2 arrowRightPosition = new Vector2(
                    linkLabel.Position.X - _arrowLeftImage.Size.X - 25f,
                    linkLabel.Position.Y + (linkLabel.Size.Y - _arrowLeftImage.Size.Y) / 2 - 10f
                );

                Vector2 arrowLeftPosition = new Vector2(
                    linkLabel.Position.X + linkLabel.Size.X + 3f,
                    linkLabel.Position.Y + (linkLabel.Size.Y - _arrowRightImage.Size.Y) / 2 - 10f
                );

                _arrowRightImage.SetPosition(arrowRightPosition);
                _arrowLeftImage.SetPosition(arrowLeftPosition);
            }
        }
        private void SetInitialControlPosition()
        {
            float bottomPosition = _gameRef.ScreenRectangle.Bottom - (_controlManager.Count * 30f);
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
        #endregion
    }
}
