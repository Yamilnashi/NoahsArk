using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities
{
    public interface IDrawableSortable
    {
        Vector2 Position { get; }
        float GetDepthY();
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
    }
}
