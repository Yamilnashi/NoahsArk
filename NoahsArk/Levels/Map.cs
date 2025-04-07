using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Levels.Maps;
using NoahsArk.Rendering;

namespace NoahsArk.Levels
{
    public class Map
    {
        #region Fields
        private EMapCode _mapCode;
        private TileMap _tileMap;
        // characters
        // enemies
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Map(TileMap tileMap)
        {
            _tileMap = tileMap;
        }
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {
            // spawn enemies
            // update spawners

            // update enemies


        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {

        }
        #endregion
    }
}
