using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Rendering;

namespace NoahsArk.Levels.Maps
{
    public interface ILayer
    {
        T GetProperty<T>(string propertyName);
        bool HasProperty(string propertyName);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera, List<TileSet> tileSets);
    }
}
