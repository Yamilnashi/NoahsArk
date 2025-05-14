using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.Menus;
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
        private Dictionary<EMenuCategoryType, MenuCategoryItem> _pauseMenuCategoryDict;
        private Dictionary<EMenuCategoryType, IMenuCategoryContent> _contentRenderers;
        private PictureBox _arrowRightImage;
        private PictureBox _arrowLeftImage;
        private int _selectedIndex = 0;
        private float _maxItemWidth = 0f;
        private Player _player;
        private Camera _camera;
        private EMapCode _currentMap;
        private EMenuCategoryType _activeCategory;
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
            _pauseMenuCategoryDict = new Dictionary<EMenuCategoryType, MenuCategoryItem>();
            
            GamePlayScreen screen = (GamePlayScreen)_gameStateManager.CurrentState;
            _currentMap = screen.World.CurrentMapCode;            
            LoadContent();
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = _gameRef.Content;
            Texture2D backgroundImage = content.Load<Texture2D>("Assets/Menus/pause-menu-book");
            Texture2D arrowRightTexture = content.Load<Texture2D>("Assets/Menus/selector_right");
            Texture2D arrowLeftTexture = content.Load<Texture2D>("Assets/Menus/selector_left");
            _backgroundImage = new PictureBox(
                backgroundImage,
                new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height)
            );
            _arrowRightImage = new PictureBox(
                arrowRightTexture,
                new Rectangle(0, 0, arrowRightTexture.Width, arrowRightTexture.Height)
            );
            _arrowLeftImage = new PictureBox(
                arrowLeftTexture,
                new Rectangle(0, 0, arrowLeftTexture.Width, arrowLeftTexture.Height)
            );
            _controlManager.Add(_backgroundImage);
            _controlManager.Add(_arrowRightImage);
            _controlManager.Add(_arrowLeftImage);

            CreateCategoryItems();


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

            Vector2 mouseScreenPosition = InputHandler.MouseAsVector2;
            bool tabClicked = InputHandler.CheckMousePress(EMouseButton.Left);

            for (int i = 0; i < _pauseMenuCategoryDict.Keys.Count; i++)
            {
                EMenuCategoryType category = _pauseMenuCategoryDict.Keys.ElementAt(i);
                MenuCategoryItem tab = _pauseMenuCategoryDict[category];
                if (tab.Bounds.Contains(mouseScreenPosition))
                {
                    if (tabClicked)
                    {
                        // deactivate all tabs
                        for (int j = 0; j < _pauseMenuCategoryDict.Keys.Count; j++)
                        {
                            EMenuCategoryType categoryToDeactivate = _pauseMenuCategoryDict.Keys.ElementAt(j);
                            MenuCategoryItem tabToDeactivate = _pauseMenuCategoryDict[categoryToDeactivate];
                            tabToDeactivate.IsActive = false;
                        }
                        tab.IsActive = true;
                        _activeCategory = category;
                    }
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _controlManager.Draw(_gameRef.SpriteBatch);
            for (int i = 0; i < _pauseMenuCategoryDict.Keys.Count; i++)
            {
                EMenuCategoryType categoryType = _pauseMenuCategoryDict.Keys.ElementAt(i);
                _pauseMenuCategoryDict[categoryType].Draw(_gameRef.SpriteBatch, gameTime);
            }

            int pageStartY = 150;
            int pageWidth = 578;
            int pageHeight = 400;
            Rectangle leftPage = new Rectangle(350, pageStartY, pageWidth, pageHeight);
            int rightPageStartX = leftPage.X + 320;
            Rectangle rightPage = new Rectangle(rightPageStartX, pageStartY, pageWidth, pageHeight);

            if (_contentRenderers.TryGetValue(_activeCategory, out IMenuCategoryContent renderer))
            {
                renderer.DrawLeftPage(_gameRef.SpriteBatch, gameTime, leftPage);
                renderer.DrawRightPage(_gameRef.SpriteBatch, gameTime, rightPage);
            }

            _gameRef.SpriteBatch.End();
        }
        #endregion

        #region Private
        private void CreateCategoryItems()
        {
            _contentRenderers = new Dictionary<EMenuCategoryType, IMenuCategoryContent>()
            {
                { EMenuCategoryType.Equipment, new EquipmentMenuContent(_player, _gameRef.Content) }
            };

            Texture2D categoryitemContainerTexture = _gameRef.Content.Load<Texture2D>("Assets/Menus/menu-category-active");

            EMenuCategoryType[] categoryTypes = Enum.GetValues(typeof(EMenuCategoryType))
                .Cast<EMenuCategoryType>()
                .ToArray();

            int positionOffsetX = 340;
            for (int i = 0; i < categoryTypes.Length; i++)
            {
                EMenuCategoryType category = categoryTypes[i];
                string iconFilePath = category.GetIconFilePath();
                Texture2D icon = _gameRef.Content.Load<Texture2D>(iconFilePath);
                Vector2 menuItemPosition = new Vector2(positionOffsetX, 106);
                MenuCategoryItem menuItem = new MenuCategoryItem(category, menuItemPosition, i == 0,
                    categoryitemContainerTexture, icon);
                _pauseMenuCategoryDict[category] = menuItem;
                positionOffsetX += categoryitemContainerTexture.Width + 3; // spacing
            }

            _activeCategory = EMenuCategoryType.Equipment;
        }

        private void CreateEquipmentSlots()
        {
            Texture2D equipmentSlotTexture = _gameRef.Content.Load<Texture2D>("Assets/Menu/menu-equipment-slot");
            Texture2D equipmentSlotSelectedTexture = _gameRef.Content.Load<Texture2D>("Assets/Menu/menu-equipment-slot-selected");
            
        }

        private LinkLabel CreateMenuOption(string optionText)
        {
            LinkLabel label = new LinkLabel("Silver", 28, optionText, Color.Black, Color.Red);
            label.Selected += new EventHandler(MenuItem_Selected);
            label.HasFocus = _currentMap.ToString().Equals(optionText, StringComparison.OrdinalIgnoreCase);
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
                _camera.LockToPosition(_player.Position, gamePlayScreen.World.CurrentMap);
                _currentMap = mapCode;
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
