using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        public void DrawLeftPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {
            spriteBatch.Draw(_equipmentSlotTexture, new Vector2(bounds.X + 10, bounds.Y + 10), Color.White);
            spriteBatch.DrawString(ControlManager.SpriteFont("Monogram", 18), "Main Hand", new Vector2(bounds.X + 50, bounds.Y + 10), Color.Black);
        }
        public void DrawRightPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds)
        {
            string stats = $"Health: {_player.HealthPoints}/{_player.MaxHealthPoints}";
            spriteBatch.DrawString(ControlManager.SpriteFont("Monogram", 18), stats, new Vector2(bounds.X + 10, bounds.Y + 10), Color.Black);
        }
        #endregion
    }
}
