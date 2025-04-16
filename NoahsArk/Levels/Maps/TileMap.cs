using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Rendering;

namespace NoahsArk.Levels.Maps
{
    public class TileMap
    {
        #region Fields
        private List<TileSet> _tileSets;
        private List<ILayer> _mapLayers;
        private int _mapWidth;
        private int _mapHeight;
        #endregion

        #region Properties
        public List<TileSet> TileSets { get { return _tileSets; } set { _tileSets = value; } }
        public List<ILayer> MapLayers { get { return _mapLayers; } set { _mapLayers = value; } }
        public int MapWidth { get { return _mapWidth; } set { _mapWidth = value; } }
        public int MapHeight { get { return _mapHeight; } set { _mapHeight = value; } }

        #endregion

        #region Constructor
        public TileMap(int mapWidth, int mapHeight, int tileWidth, int tileHeight, List<ILayer> mapLayers, List<TileSet> tileSets)
        {
            _mapLayers = mapLayers;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _tileSets = tileSets;
        }
        #endregion

        #region Methods
        public void AddLayer(ILayer layer)
        {
            if (layer is MapLayer)
            {
                if (!IsMapLayerSameSize((MapLayer)layer))
                {
                    throw new Exception("Map layer size exception.");
                }
            }
            _mapLayers.Add(layer);
        }

        public void AddTileSet(TileSet tileSet)
        {
            _tileSets.Add(tileSet);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _mapLayers.Count; i++)
            {
                ILayer layer = _mapLayers[i];
                layer.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            List<ILayer> layersToDrawAfterCharacter = new List<ILayer>();   
            for (int i = 0; i < _mapLayers.Count;i++)
            {
                ILayer layer = _mapLayers[i];
                if (layer is null)
                {
                    continue;
                }

                if (layer.HasProperty("drawAfterCharacter") &&
                    layer.GetProperty<bool>("drawAfterCharacter"))
                {
                    layersToDrawAfterCharacter.Add(layer);
                    continue;
                }
                layer.Draw(spriteBatch, gameTime, camera, _tileSets);
            }

            // enemies draw?

            // draw player

            for (int i = 0; i < layersToDrawAfterCharacter.Count; i++)
            {
                ILayer layer = layersToDrawAfterCharacter[i];
                layer.Draw(spriteBatch, gameTime, camera, _tileSets);
            }
        }

        #endregion

        #region Private
        private bool IsMapLayerSameSize(MapLayer mapLayer)
        {
            return mapLayer.Width == _mapWidth &&
                    mapLayer.Height == _mapHeight;
        }
        #endregion
    }
}
