using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Rendering;

namespace NoahsArk.Levels.Maps
{
    public class MapLayer : ILayer
    {
        #region Fields
        private string _name;
        private int[,] _tiles;
        private EMapLayerType _mapLayerType;
        private Dictionary<string, object> _properties;
        #endregion
        #region Properties
        public string Name { get { return _name; } set { _name = value; } }
        public int Width
        {
            get 
            { 
                return _tiles.GetLength(1);
            }
        }
        public int Height
        {
            get
            {
                return _tiles.GetLength(0);
            }
        }
        #endregion
        #region Constructor
        public MapLayer(string name, int[,] tiles, Dictionary<string, object> properties)
        {
            _name = name;
            _tiles = tiles;
            _properties = properties;
        }
        #endregion

        #region Methods
        public int GetTile(int x, int y)
        {
            return _tiles[y, x];
        }
        public void SetTile(int x, int y, int value)
        {
            _tiles[x, y] = value;
        }
        #endregion

        #region Interface Implementation Methods      
        public T GetProperty<T>(string propertyName)
        {
            if (_properties.TryGetValue(propertyName, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            throw new KeyNotFoundException($"Property {propertyName} not found or not of type {typeof(T)}");
        }

        public bool HasProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName);
        }

        public void Update(GameTime gameTime)
        {
            // todo:
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera, List<TileSet> tileSets)
        {
            Point cameraPoint = camera.CameraPoint();
            Point viewPoint = camera.ViewPoint();   

            Point min = new Point();
            Point max = new Point();

            min.X = Math.Max(0, cameraPoint.X - 1);
            min.Y = Math.Max(0, cameraPoint.Y - 1);
            max.X = Math.Min(viewPoint.X + 1, Width);
            max.Y = Math.Min(viewPoint.Y + 1, Height);

            Rectangle destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);

            for (int y = min.Y; y < max.Y; y++)
            {
                destination.Y = y * Engine.TileHeight;

                for (int x = min.X; x < max.X; x++)
                {
                    int tileIndex = GetTile(x, y);

                    // skip drawing if TileIndex is invalid or represents an empty tile
                    if (tileIndex <= 0)
                    {
                        continue; // we are assuming that 0 or negative numbers represents empty tiles
                    }

                    destination.X = x * Engine.TileWidth;

                    if (tileSets.Count > 0)
                    {
                        foreach (TileSet tileSet in tileSets)
                        {
                            if (IsTileIndexBelongInTileset(tileIndex, tileSet))
                            {
                                int localId = tileIndex - tileSet.FirstGid;
                                Rectangle sourceRect = tileSet.GetCurrentSourceRectangle(localId, gameTime);

                                if (sourceRect != Rectangle.Empty)
                                {
                                    spriteBatch.Draw(tileSet.Texture, destination, sourceRect, Color.White);
                                }
                                break; // we have found the correct tileset for this tile, we can break the loop
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Private
        private bool IsTileIndexBelongInTileset(int tileIndex, TileSet tileSet)
        {
            if (tileIndex >= tileSet.FirstGid)
            {
                int maximumTileIndex = tileSet.FirstGid + tileSet.TileCount;
                return tileIndex < maximumTileIndex;
            }
            return false;
        }
        #endregion
    }
}
