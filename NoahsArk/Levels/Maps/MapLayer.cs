using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities;
using NoahsArk.Rendering;

namespace NoahsArk.Levels.Maps
{
    public class MapLayer : ILayer
    {
        #region Fields
        private string _name;
        private long[,] _tiles;
        private EMapLayerType _mapLayerType;
        private Dictionary<string, object> _properties;
        private float _offsetX;
        private float _moveSpeed;
        private float _opacity;
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
        public MapLayer(string name, long[,] tiles, float opacity, Dictionary<string, object> properties)
        {
            _name = name;
            _tiles = tiles;
            _properties = properties;
            _offsetX = 0f;
            _moveSpeed = 2f;
            _opacity = opacity;
            if (properties.TryGetValue("weatherSpeed", out object speed))
            {
                _moveSpeed = float.Parse(speed.ToString());
            }
        }
        #endregion

        #region Methods
        public long GetTile(int x, int y)
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
            if (HasProperty("weatherTypeCode"))
            {
                string typeString = GetProperty<string>("weatherTypeCode").ToString();
                EWeatherType type = (EWeatherType)Enum.Parse(typeof(EWeatherType), typeString, true);
                UpdateWeather(gameTime, type);                
            }
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

            if (HasProperty("weatherTypeCode"))
            {
                DrawMovingLayer(spriteBatch, tileSets, gameTime, min, max);
            }
            else
            {
                DrawStaticLayer(spriteBatch, tileSets, gameTime, min, max);
            }
        }
        #endregion
        #region Private
        private void UpdateWeather(GameTime gameTime, EWeatherType weatherType)
        {
            if (weatherType == EWeatherType.Clouds)
            {
                _offsetX -= _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                float layerWidthInPixels = Width * Engine.TileWidth;
                if (Math.Abs(_offsetX) >= layerWidthInPixels)
                {
                    _offsetX += layerWidthInPixels; // reset the layer to start position
                }
            }
        }
        private bool IsTileIndexBelongInTileset(long tileIndex, TileSet tileSet)
        {
            if (tileIndex >= tileSet.FirstGid)
            {
                int maximumTileIndex = tileSet.FirstGid + tileSet.TileCount;
                return tileIndex < maximumTileIndex;
            }
            return false;
        }

        private void DrawMovingLayer(SpriteBatch spriteBatch, List<TileSet> tileSets, GameTime gameTime, Point min, Point max)
        {
            float layerWidthInPixels = Width * Engine.TileWidth;

            // calculate extra tiles needed based on the maximum offset
            int extraTiles = (int)Math.Ceiling(layerWidthInPixels / Engine.TileWidth);
            int extendedMinX = min.X - extraTiles;
            int extendedMaxX = max.X + extraTiles;

            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = extendedMinX; x < extendedMaxX; x++)
                {
                    // wrap the x-coordinate to get the correct tile index
                    int wrappedX = (x % Width + Width) % Width;
                    long tileIndex = GetTile(wrappedX, y);

                    // Skip drawing if TileIndex is invalid or represents an empty tile
                    if (tileIndex <= 0)
                    {
                        continue; // Assuming 0 or negative numbers represent empty tiles
                    }

                    // Calculate the base position with the floating-point offset
                    float baseX = x * Engine.TileWidth + _offsetX;
                    float baseY = y * Engine.TileHeight;

                    // Draw the tile at its current position
                    Vector2 position = new Vector2(baseX, baseY);
                    DrawTileWithPosition(spriteBatch, tileSets, tileIndex, position, gameTime);
                }
            }
        }

        private void DrawStaticLayer(SpriteBatch spriteBatch, List<TileSet> tileSets, GameTime gameTime, Point min, Point max)
        {
            Rectangle destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);

            for (int y = min.Y; y < max.Y; y++)
            {
                destination.Y = y * Engine.TileHeight;

                for (int x = min.X; x < max.X; x++)
                {
                    long tileIndex = GetTile(x, y);

                    // Skip drawing if TileIndex is invalid or represents an empty tile
                    if (tileIndex <= 0)
                    {
                        continue; // Assuming 0 or negative numbers represent empty tiles
                    }

                    destination.X = x * Engine.TileWidth;

                    DrawTile(spriteBatch, tileSets, tileIndex, destination, gameTime);
                }
            }
        }

        private void DrawTileWithPosition(SpriteBatch spriteBatch, List<TileSet> tileSets, long tileIndex, Vector2 position, GameTime gameTime)
        {
            if (tileSets.Count > 0)
            {
                foreach (TileSet tileSet in tileSets)
                {
                    if (IsTileIndexBelongInTileset(tileIndex, tileSet))
                    {
                        long localId = tileIndex - tileSet.FirstGid;
                        Rectangle sourceRect = tileSet.GetCurrentSourceRectangle(localId, gameTime);

                        if (sourceRect != Rectangle.Empty)
                        {
                            // Use Vector2 position for smooth movement, scale to maintain tile size
                            spriteBatch.Draw(tileSet.Texture, position, sourceRect, Color.White * _opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        break; // Found the correct tileset for this tile, break the loop
                    }
                }
            }
        }

        private void DrawTile(SpriteBatch spriteBatch, List<TileSet> tileSets, long tileIndex, Rectangle destination, GameTime gameTime)
        {
            if (tileSets.Count > 0)
            {
                foreach (TileSet tileSet in tileSets)
                {
                    if (IsTileIndexBelongInTileset(tileIndex, tileSet))
                    {
                        long localId = tileIndex - tileSet.FirstGid;
                        Rectangle sourceRect = tileSet.GetCurrentSourceRectangle(localId, gameTime);

                        if (sourceRect != Rectangle.Empty)
                        {
                            spriteBatch.Draw(tileSet.Texture, destination, sourceRect, Color.White * _opacity);
                        }
                        break; // we have found the correct tileset for this tile, we can break the loop
                    }
                }
            }
        }
        #endregion
    }
}
