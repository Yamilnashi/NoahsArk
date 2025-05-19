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
        private PictureBox _backgroundImage;
        private Dictionary<EMenuCategoryType, MenuCategoryItem> _pauseMenuCategoryDict;
        private Dictionary<EMenuCategoryType, IMenuCategoryContent> _contentRenderers;
        private Player _player;
        private EMenuCategoryType _activeCategory;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public PauseMenuScreen(Game game, GameStateManager manager, Player player, Camera camera) : base(game, manager)
        {
            _player = player;
            _pauseMenuCategoryDict = new Dictionary<EMenuCategoryType, MenuCategoryItem>();            
            GamePlayScreen screen = (GamePlayScreen)_gameStateManager.CurrentState;      
            LoadContent();
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            ContentManager content = _gameRef.Content;
            Texture2D backgroundImage = content.Load<Texture2D>("Assets/Menus/pause-menu-book");
            _backgroundImage = new PictureBox(
                backgroundImage,
                new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height)
            );
            _controlManager.Add(_backgroundImage);          

            CreateCategoryItems();

            if (_contentRenderers.TryGetValue(_activeCategory, out var renderer))
            {
                _contentRenderers[_activeCategory].LoadContent(_gameStateManager, _gameRef.Content, _controlManager);
            }            
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
            if (_contentRenderers.TryGetValue(_activeCategory, out IMenuCategoryContent renderer))
            {
                renderer.Update(gameTime);
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
                { EMenuCategoryType.Equipment, new EquipmentMenuContent(_player) },
                { EMenuCategoryType.Map, new MapMenuContent(_player) },
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
        #endregion
    }
}
