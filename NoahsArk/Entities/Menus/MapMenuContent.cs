using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Levels;
using NoahsArk.Managers;
using NoahsArk.States;

namespace NoahsArk.Entities.Menus
{
    public class MapMenuContent : IMenuCategoryContent
    {
        #region Fields
        private readonly Player _player;
        private ControlManager _controlManager;
        private GameStateManager _gameStateManager;
        private PictureBox _arrowRightImage;
        private PictureBox _arrowLeftImage;
        private float _maxItemWidth = 0f;
        #endregion

        #region Properties
        public EMenuCategoryType CategoryType => EMenuCategoryType.Map;
        #endregion

        #region Constructor
        public MapMenuContent(Player player)
        {
            _player = player;
        }
        #endregion

        #region Methods
        public void LoadContent(GameStateManager gameStateManager, ContentManager content, ControlManager controlManager)
        {
            EMapCode[] mapCodes = Enum.GetValues(typeof(EMapCode))
                .Cast<EMapCode>()
                .ToArray();
            _gameStateManager = gameStateManager;
            _controlManager = controlManager;

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

            string[] menuOptions = mapCodes.Select(x => x.ToString()).ToArray();
            for (int i = 0; i < menuOptions.Length; i++)
            {
                LinkLabel option = CreateMenuOption(menuOptions[i]);
                _controlManager.Add(option);
            }

            _controlManager.FocusChanged += new EventHandler(ControlManager_FocusChanged);
            SetInitialControlPosition();
            ControlManager_FocusChanged(_controlManager
                .Where(x => !string.IsNullOrEmpty(x.Text))
                .FirstOrDefault(x => x.Text
                                    .Equals(_player.CurrentMap.ToString(), StringComparison.OrdinalIgnoreCase)),
                                    null);
        }
        public void Update(GameTime gameTime)
        {
        }
        public void DrawLeftPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {
        }
        public void DrawRightPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {
        }
        #endregion

        #region Private
        private LinkLabel CreateMenuOption(string optionText)
        {
            LinkLabel label = new LinkLabel("Silver", 28, optionText, Color.Black, Color.Red);
            label.Selected += new EventHandler(MenuItem_Selected);
            label.HasFocus = _player.CurrentMap.ToString().Equals(optionText, StringComparison.OrdinalIgnoreCase);
            return label;
        }
        private void MenuItem_Selected(object sender, EventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            EMapCode mapCode = (EMapCode)Enum.Parse(typeof(EMapCode), label.Text, true);
            if (_gameStateManager.CurrentState is GamePlayScreen gamePlayScreen)
            {
                gamePlayScreen.World.CurrentMap.RemovePlayer(_player);
                gamePlayScreen.World.SetCurrentMap(mapCode);
                gamePlayScreen.World.CurrentMap.AddPlayer(_player);                
                _player.Camera.LockToPosition(_player.Position, gamePlayScreen.World.CurrentMap);
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
            // Calculate the left page bounds (approximate)
            float leftPageStartX = 225f; // 10% of 1280
            float leftPageWidth = 512f;  // Half of the content width (80% of 1280)
            float leftPageCenterX = leftPageStartX + (leftPageWidth / 2f); // Center of the left page
            float startY = 72f + 200f; // 10% of 720 + padding
            float currentY = startY;

            foreach (Control control in _controlManager)
            {
                if (control is LinkLabel label)
                {
                    if (control.Size.X > _maxItemWidth)
                    {
                        _maxItemWidth = control.Size.X;
                    }
                    // Center the label horizontally in the left page
                    label.Position = new Vector2(
                        leftPageCenterX - (label.Size.X / 2f),
                        currentY
                    );
                    currentY += label.Size.Y + 5f; // Space between items
                }
            }
        }
        #endregion
    }
}
