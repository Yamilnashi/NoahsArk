using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Levels.Maps
{
    public class TileSet
    {
        #region Fields
        private Texture2D _texture;
        private string _source;
        private int _firstGid;
        private int _tileWidth;
        private int _tileHeight;
        private int _columns;
        private int _tileCount;
        private Rectangle[] _sourceRectangles;
        //private Dictionary<int, AnimatedTile> _animatedTiles;
        #endregion

        #region Properties
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public string Source { get { return _source; } set { _source = value; } }
        public int FirstGid { get { return _firstGid; } set { _firstGid = value; } }
        public int TileWidth { get { return _tileWidth; } set { _tileWidth = value; } }
        public int TileHeight { get { return _tileHeight; } set { _tileHeight = value; } }
        public int Columns { get { return _columns; } set { _columns = value; } }
        public int TileCount { get { return _tileCount; } set { _tileCount = value; } }
        public Rectangle[] SourceRectangles { get { return (Rectangle[])_sourceRectangles.Clone(); } }
        #endregion

        #region Constructor
        public TileSet(string source, int firstGid, int tileWidthInPixels, int tileHeightInPixels)
        {
            _firstGid = firstGid;
            _tileWidth = tileWidthInPixels; 
            _tileHeight = tileHeightInPixels;   
            _source = source;
        }
        #endregion

        #region Methods
        public void LoadTileSetTexture(ContentManager content)
        {
            string path = $"Tiled/{Path.GetFileNameWithoutExtension(_source)}";
            Texture2D tilesetTexture = content.Load<Texture2D>(path);
            Texture = tilesetTexture;
        }

        public void LoadTileSetData()
        {
            string xmlPath = $"Content/Tiled/{_source}";
            // use XML parsing to read the file
            using (var reader = XmlReader.Create(xmlPath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.Equals("tileset", StringComparison.OrdinalIgnoreCase) &&
                            reader.GetAttribute("tilecount") != null)
                        {
                            ReadTileSetElement(reader);
                            continue;
                        }

                        if (reader.Name.Equals("tile", StringComparison.OrdinalIgnoreCase) &&
                            reader.GetAttribute("id") != null)
                        {
                            ReadTileElement(reader);
                            continue;
                        }
                    }
                }
            }
        }

        public Rectangle GetCurrentSourceRectangle(int tileId, GameTime gameTime)
        {
            if (tileId >= _tileCount)
            {
                return Rectangle.Empty;
            }

            //if (AnimatedTiles.TryGetValue(tileId, out AnimatedTile animatedTile))
            //{
            //    int totalElapsedMilliseconds = (int)gameTime.TotalGameTime.TotalMilliseconds;
            //    int totalDuration = animatedTile.Frames.Sum(f => f.Duration);
            //    int currentCycleTime = totalElapsedMilliseconds % totalDuration;
            //    int accumulatedTime = 0;
            //    foreach (var frame in animatedTile.Frames)
            //    {
            //        accumulatedTime += frame.Duration;
            //        if (currentCycleTime < accumulatedTime)
            //        {
            //            // If the duration of frames is off, adjust here:
            //            //int adjustedTileId = frame.TileId; // This would be frame.TileId if each frame is exactly 300ms
            //            // But if you want to ensure each frame lasts 300ms, adjust like this:
            //            int frameIndex = (int)Math.Floor((double)currentCycleTime / 150);
            //            //return sourceRectangles[adjustedTileId];
            //            return sourceRectangles[animatedTile.Frames[frameIndex % animatedTile.Frames.Count].TileId];
            //        }
            //    }
            //}
            return _sourceRectangles[tileId];
        }

        public void SetSourceRectangles()
        {
            _sourceRectangles = new Rectangle[_tileCount];
            for (int tile = 0; tile < _tileCount; tile++)
            {
                int x = (tile % _columns) * _tileWidth;
                int y = (tile / _columns) * _tileHeight;

                // create new rectangle for each tile in this tileset
                _sourceRectangles[tile] = new Rectangle(
                        x,
                        y,
                        _tileWidth,
                        _tileHeight
                    );
            }
        }
        #endregion

        #region Private
        private void ReadTileSetElement(XmlReader reader)
        {
            int tileCount = int.Parse(reader.GetAttribute("tilecount"));
            int columns = int.Parse(reader.GetAttribute("columns"));
            _tileCount = tileCount;
            _columns = columns;
        }

        private void ReadTileElement(XmlReader reader)
        {
            int tileId = int.Parse(reader.GetAttribute("id"));
            if (reader.ReadToFollowing("animation"))
            {
                ReadTileAnimation(reader);
            }
        }

        private void ReadTileAnimation(XmlReader reader)
        {
            //AnimatedTile animation = new AnimatedTile();
            //while (reader.Read() && 
            //    reader.NodeType != XmlNodeType.EndElement)
            //{
            //    if (reader.Name.Equals("frame", StringComparison.OrdinalIgnoreCase))
            //    {
            //        animation.Frames.Add(new AnimationFrame
            //        {
            //            TileId = int.Parse(reader.GetAttribute("tileid")),
            //            Duration = int.Parse(reader.GetAttribute("duration"))
            //        });
            //    }
            //}
            //AnimatedTiles.Add(animation);
        }
        #endregion
    }
}
