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
        private readonly Texture2D _equipmentSlotTexture;
        #endregion

        #region Properties
        public EMenuCategoryType CategoryType => EMenuCategoryType.Equipment;
        #endregion

        #region Constructor
        public EquipmentMenuContent(Player player, ContentManager content)
        {
            _player = player;
            _equipmentSlotTexture = content.Load<Texture2D>("Assets/Menus/menu-equipment-slot");
        }
        #endregion

        #region Methods        
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
            int animatedSpritePositionX = bounds.X + (bounds.Width / 4) - 32; // 16px = width of the player sprite
            int animatedSpritePositionY = bounds.Y + (bounds.Height / 2) - 160;
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
            string stats = $"Health: {_player.HealthPoints}/{_player.MaxHealthPoints}";
            spriteBatch.DrawString(ControlManager.SpriteFont("Monogram", 18), stats, new Vector2(bounds.X + 10, bounds.Y + 10), Color.Black);
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
                    texturePosition = new Vector2(texturePosition.X + 10, texturePosition.Y + 250);
                    textPosition = new Vector2(textPosition.X + 50, textPosition.Y + 260);
                    slotText = "Main Hand";
                    break;
                case EEquipmentSlot.OffHand:
                    texturePosition = new Vector2(texturePosition.X + 216, texturePosition.Y + 250);
                    textPosition = new Vector2(textPosition.X + 216, textPosition.Y + 260);
                    slotText = "Off Hand";
                    break;
                case EEquipmentSlot.Head:
                    texturePosition = new Vector2(texturePosition.X + 110, texturePosition.Y + 160);
                    textPosition = new Vector2(textPosition.X + 80, textPosition.Y + 170);
                    slotText = "Helmet";
                    break;
                default:
                    return;
            }

            spriteBatch.Draw(texture, texturePosition, Color.White);
            spriteBatch.DrawString(font, slotText, textPosition, Color.Black);

        }
        #endregion
    }
}
