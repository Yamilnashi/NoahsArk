using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Extensions;
using NoahsArk.Rendering;
using NoahsArk.Utilities;

namespace NoahsArk.Levels.Maps
{
    public class World : DrawableGameComponent
    {
        #region Fields
        private Game _gameRef;
        private Dictionary<EMapCode, Map> _maps = new Dictionary<EMapCode, Map>();
        private EMapCode _currentMap = EMapCode.Development;
        #endregion

        #region Properties
        public Map CurrentMap
        {
            get
            {
                return _maps[_currentMap];
            }
        }
        #endregion

        #region Constructor
        public World(Game game) : base(game)
        {
            _gameRef = game;
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            string filePath = _currentMap.TileMapFilePath();
            TileMapReader tmr = new TileMapReader(filePath);
            tmr.ProcessTileMap();
            TileMap tileMap = tmr.CreateTileMap();
            ConfigureTileSetsInMap(tileMap);
            CreateMap(tileMap);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CurrentMap.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            CurrentMap.Draw(spriteBatch, gameTime, camera);
        }
        #endregion

        #region Private
        private void ConfigureTileSetsInMap(TileMap tileMap)
        {
            for (int i = 0; i < tileMap.TileSets.Count; i++)
            {
                TileSet tileSet = tileMap.TileSets[i];
                tileSet.LoadTileSetTexture(_gameRef.Content);
                tileSet.LoadTileSetData();
                tileSet.SetSourceRectangles();
            }
        }

        private void CreateMap(TileMap tileMap)
        {
            Map map = new Map(tileMap);
        }
        #endregion
    }
}
