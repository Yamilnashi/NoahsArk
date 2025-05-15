using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities.Menus
{
    public interface IMenuCategoryContent
    {
        EMenuCategoryType CategoryType { get; }
        void Update(GameTime gameTime);
        void DrawLeftPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds);
        void DrawRightPage(SpriteBatch spriteBatch, GameTime gameTime, Rectangle bounds);
    }
}
