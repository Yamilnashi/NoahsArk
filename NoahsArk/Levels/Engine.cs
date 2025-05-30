﻿using Microsoft.Xna.Framework;

namespace NoahsArk.Levels
{
    public class Engine
    {
        #region Fields
        private static int _tileWidth;
        private static int _tileHeight;
        #endregion

        #region Properties
        public static int TileWidth { get { return _tileWidth; } }
        public static int TileHeight { get { return _tileHeight; } }
        #endregion

        #region Constructor
        public Engine(int tileWidth, int tileHeight)
        {
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
        }
        #endregion

        #region Methods
        public static Point VectorToCell(Vector2 position)
        {
            return new Point((int)position.X / _tileWidth, (int)position.Y / _tileHeight);
        }
        #endregion
    }
}
