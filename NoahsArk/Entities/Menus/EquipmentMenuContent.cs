using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using NoahsArk.Managers;

namespace NoahsArk.Entities.Menus
{
    public class EquipmentMenuContent : IMenuCategoryContent
    {
        #region Fields
        private readonly Player _player;
        private Texture2D _equipmentSlotTexture;
        private Texture2D _equipmentLargeSlotTexture;
        #endregion

        #region Properties
        public EMenuCategoryType CategoryType => EMenuCategoryType.Equipment;
        #endregion

        #region Constructor
        public EquipmentMenuContent(Player player)
        {
            _player = player;            
        }
        #endregion

        #region Methods        
        public void LoadContent(GameStateManager gameStateManager,ContentManager content, ControlManager controlManager)
        {
            _equipmentSlotTexture = content.Load<Texture2D>("Assets/Menus/menu-equipment-slot");
            _equipmentLargeSlotTexture = content.Load<Texture2D>("Assets/Menus/menu-equipment-large-slot");
        }
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _player.Animations[EAnimationKey.Idle][EDirection.Down].Keys.Count; i++)
            {
                EEquipmentSlot slot = _player.Animations[EAnimationKey.Idle][EDirection.Down].Keys.ElementAt(i);
                _player.Animations[EAnimationKey.Idle][EDirection.Down][slot].Update(gameTime, EAnimationKey.Idle, 2f);
            }
        }
        
        public void DrawLeftPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {
            SpriteFont font = ControlManager.SpriteFont("Monogram", 18);
            string stats = $"HP: {_player.HealthPoints}/{_player.MaxHealthPoints}";
            spriteBatch.DrawString(font, stats, new Vector2(bounds.X, bounds.Y), Color.Black);

            int animatedSpritePositionX = bounds.X + (bounds.Width / 4) - 32; // 16px = width of the player sprite
            int animatedSpritePositionY = bounds.Y + (bounds.Height / 2) - 190;
            Vector2 animatedSpritePosition = new Vector2(animatedSpritePositionX, animatedSpritePositionY);

            EEquipmentSlot[] slots = Enum.GetValues(typeof(EEquipmentSlot))
                .Cast<EEquipmentSlot>()
                .ToArray();

            for (int i = 0; i < slots.Length; i++)
            {
                EEquipmentSlot slot = slots[i];
                DrawEquipmentSlot(spriteBatch, bounds, slot, font);
            }

            for (int i = 0; i < _player.Animations[EAnimationKey.Idle][EDirection.Down].Keys.Count; i++)
            {
                EEquipmentSlot slot = _player.Animations[EAnimationKey.Idle][EDirection.Down].Keys.ElementAt(i);
                _player.Animations[EAnimationKey.Idle][EDirection.Down][slot].Draw(spriteBatch, animatedSpritePosition, EDirection.Down, Color.White, 3f);
            }                       
        }
        public void DrawRightPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {            
            SpriteFont font = ControlManager.SpriteFont("Monogram", 18);
            string inventoryStats = $"Inventory Slots: 10/10";
            spriteBatch.DrawString(font, inventoryStats, new Vector2(bounds.X, bounds.Y), Color.Black);
            Vector2 slotPosition = new Vector2(bounds.X, bounds.Y + 30);
            int maxInventorySlots = 10;
            int maxPerRow = 5;
            decimal rowsDecimal = maxInventorySlots / maxPerRow;
            int rows = (int)Math.Ceiling(rowsDecimal);

            int totalDrawnSlots = 0;
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int slotIndex = 0; slotIndex < maxPerRow; slotIndex++)
                {
                    if (totalDrawnSlots == maxInventorySlots)
                    {
                        break;
                    }
                    DrawInventorySlot(spriteBatch, bounds, slotPosition, rowIndex, slotIndex);
                    slotPosition.X += 50;
                    totalDrawnSlots++;
                }
                slotPosition.X = bounds.X;
                slotPosition.Y += 60;
            }
        }
        #endregion

        #region Private
        private void DrawEquipmentSlot(SpriteBatch spriteBatch, Rectangle bounds, EEquipmentSlot slot, SpriteFont font)
        {
            Texture2D texture = _equipmentSlotTexture;
            Vector2 texturePosition = new Vector2(bounds.X, bounds.Y);
            Vector2 textPosition = new Vector2(bounds.X, bounds.Y);
            string slotText = "";
            switch (slot)
            {
                case EEquipmentSlot.MainHand:
                    texture = _equipmentLargeSlotTexture;
                    texturePosition = new Vector2(texturePosition.X + 10, texturePosition.Y + 200);
                    textPosition = new Vector2(textPosition.X - 7, textPosition.Y + 180);
                    slotText = "Main Hand";
                    break;
                case EEquipmentSlot.OffHand:
                    texture = _equipmentLargeSlotTexture;
                    texturePosition = new Vector2(texturePosition.X + 216, texturePosition.Y + 200);
                    textPosition = new Vector2(textPosition.X + 207, textPosition.Y + 180);
                    slotText = "Off Hand";
                    break;
                case EEquipmentSlot.Head:
                    texturePosition = new Vector2(texturePosition.X + 115, texturePosition.Y + 130);
                    textPosition = new Vector2(textPosition.X + 112, textPosition.Y + 110);
                    slotText = "Helmet";
                    break;
                case EEquipmentSlot.Feet:
                    texturePosition = new Vector2(texturePosition.X + 216, texturePosition.Y + 310);
                    textPosition = new Vector2(textPosition.X + 220, textPosition.Y + 290);
                    slotText = "Feet";
                    break;
                case EEquipmentSlot.Gloves:
                    texturePosition = new Vector2(texturePosition.X + 10, texturePosition.Y + 310);
                    textPosition = new Vector2(textPosition.X + 5, textPosition.Y + 290);
                    slotText = "Gloves";
                    break;
                case EEquipmentSlot.Chest:
                    texture = _equipmentLargeSlotTexture;
                    texturePosition = new Vector2(texturePosition.X + 115, texturePosition.Y + 200);
                    textPosition = new Vector2(textPosition.X + 115, textPosition.Y + 180);
                    slotText = "Chest";
                    break;
                case EEquipmentSlot.Legs:
                    texture = _equipmentLargeSlotTexture;
                    texturePosition = new Vector2(texturePosition.X + 115, texturePosition.Y + 310);
                    textPosition = new Vector2(textPosition.X + 120, textPosition.Y + 290);
                    slotText = "Legs";
                    break;
                default:
                    return;
            }

            spriteBatch.Draw(texture, texturePosition, Color.White);
            spriteBatch.DrawString(font, slotText, textPosition, Color.Black);

        }

        private void DrawInventorySlot(SpriteBatch spriteBatch, Rectangle bounds, Vector2 slotPosition, int rowIndex, int slotIndex)
        {
            spriteBatch.Draw(_equipmentSlotTexture, slotPosition, Color.White);
        }
        #endregion
    }
}
