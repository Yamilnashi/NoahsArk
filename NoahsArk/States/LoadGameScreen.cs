using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Extensions;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public class LoadGameScreen : BaseGameState
    {
        #region Fields
        private const int _saveSlotCount = 3;
        private PictureBox _backgroundImage;
        private Texture2D _slotContainerTexture;
        private Dictionary<int, (PictureBox, LinkLabel, bool)> _slotsDict = new();
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public LoadGameScreen(Game game, GameStateManager manager) : base(game, manager)
        {
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            _slotContainerTexture = _gameRef.Content.Load<Texture2D>("Assets/Menus/large-content-holder");
            _backgroundImage = new PictureBox(_gameRef.Content.Load<Texture2D>("Assets/Backgrounds/title_background"), _gameRef.ScreenRectangle);
            _controlManager.Add(_backgroundImage);
            AddSlotContainers();
            _controlManager.NextControl();
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
        private void AddSlotContainers()
        {
            int sumOfContainerWidths = (_gameRef.ScreenRectangle.Width - (_slotContainerTexture.Width * 3));
            int containerOffsets = sumOfContainerWidths / (_saveSlotCount + 1);
            Vector2 centered = _gameRef.ScreenRectangle.GetCenter();
            int positionOffsetX = containerOffsets;
            int positionOffsetY = (int)centered.Y - (_slotContainerTexture.Height / 2);
            for (int i = 0; i < _saveSlotCount; i++)
            {
                int slotNumber = i + 1;
                Rectangle rectangle = new Rectangle(
                        positionOffsetX,
                        positionOffsetY,
                        _slotContainerTexture.Width,
                        _slotContainerTexture.Height
                    );
                PictureBox slot = new PictureBox(_slotContainerTexture, rectangle);                
                _controlManager.Add(slot);

                string slotText = $"Slot {slotNumber}";
                Label label = new Label("Silver", 28, slotText);
                label.Position = new Vector2(
                    rectangle.X + (rectangle.Width - label.Size.X) / 2,
                    positionOffsetY + label.Size.Y / 2
                    );
                _controlManager.Add(label);
                _slotsDict[slotNumber] = (slot, null, false);
                CheckHasDataSlot(slotNumber, rectangle, positionOffsetY);
                positionOffsetX += containerOffsets + _slotContainerTexture.Width;
            }
        }

        private void CheckHasDataSlot(int slotNumber, Rectangle rectangle, int positionOffsetY)
        {
            (PictureBox container, LinkLabel label, bool hasData) = _slotsDict[slotNumber];
            if (!hasData)
            {
                LinkLabel lbl = new LinkLabel("Silver", 28, " - New Game - ", Color.Brown, Color.Black);
                lbl.PropertiesDict["slot"] = slotNumber;
                lbl.Position = new Vector2(
                        rectangle.X + (rectangle.Width - lbl.Size.X) / 2,
                        positionOffsetY + (_slotContainerTexture.Height / 2) - lbl.Size.Y / 2
                    );
                lbl.Selected += new EventHandler(DataSlot_Selected);
                label = lbl;
                _controlManager.Add(label);                
                return;
            }

            // todo: get the data and show it in the container
        }
        private void DataSlot_Selected(object sender, EventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            int slotNumber = (int)label.PropertiesDict["slot"];
            
            if (label.PropertiesDict.TryGetValue("data", out object d) && d != null)
            {
                LoadGame(slotNumber, d);
            }
            else
            {
                NewGame(slotNumber);
            }
            
        }
        private void LoadGame(int slotNumber, object data)
        {
            // todo: load the game
        }
        private void NewGame(int slotNumber)
        {
            Transition(EChangeType.Push, _gameRef.GamePlayScreen);
        }
        #endregion

    }
}
