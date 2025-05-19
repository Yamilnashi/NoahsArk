using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Managers;

namespace NoahsArk.Entities.Menus
{
    public interface IMenuCategoryContent
    {
        EMenuCategoryType CategoryType { get; }
        void LoadContent(GameStateManager gameStateManager, ContentManager content, ControlManager controlManager);
        void Update(GameTime gameTime);
        void DrawLeftPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds);
        void DrawRightPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds);
    }
}
