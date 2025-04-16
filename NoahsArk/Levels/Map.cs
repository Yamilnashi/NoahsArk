using System.Collections.Generic;
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
            for (int i = 0; i < _tileMap.MapLayers.Count; i++)
            {
                ILayer layer = _tileMap.MapLayers[i];
                layer.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            List<ILayer> layersToDrawAfterCharacter = new List<ILayer>();
            for (int i = 0; i < _tileMap.MapLayers.Count; i++)
            {
                ILayer layer = _tileMap.MapLayers[i];
                if (layer == null)
                {
                    continue;
                }

                if (layer.HasProperty("drawAfterCharacter") &&
                layer.GetProperty<bool>("drawAfterCharacter"))
                {
                    layersToDrawAfterCharacter.Add(layer);
                    continue;
                }

                layer.Draw(spriteBatch, gameTime, camera, _tileMap.TileSets);
            }

            // draw enemies

            // draw character

            for (int i = 0; i <  layersToDrawAfterCharacter.Count; i++)
            {
                ILayer layer = layersToDrawAfterCharacter[i];
                layer.Draw(spriteBatch, gameTime, camera, _tileMap.TileSets);
            }

            // draw collisions
        }
        #endregion
    }
}
