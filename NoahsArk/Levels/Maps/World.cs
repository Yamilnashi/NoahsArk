using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities.Enemies;
using NoahsArk.Extensions;
using NoahsArk.Rendering;
using NoahsArk.Utilities;

namespace NoahsArk.Levels.Maps
{
    public class World : DrawableGameComponent
    {
        #region Fields
        private Game1 _gameRef;
        private Dictionary<EMapCode, Map> _maps = new Dictionary<EMapCode, Map>();
        private EMapCode _currentMap;
        private Texture2D _debugTexture;
        #endregion

        #region Properties
        public Map CurrentMap
        {
            get
            {
                return _maps[_currentMap];
            }
        }
        public EMapCode CurrentMapCode { get { return _currentMap; } }  
        #endregion

        #region Constructor
        public World(Game1 game, Texture2D debugTexture) : base(game)
        {
            _gameRef = game;
            _debugTexture = debugTexture;
            EMapCode[] codes = Enum.GetValues(typeof(EMapCode)).Cast<EMapCode>().ToArray();
            for (int i = 0; i < codes.Length; i++)
            {
                EMapCode mapCode = codes[i];
                string filePath = mapCode.TileMapFilePath();
                TileMapReader tmr = new TileMapReader(filePath);
                tmr.ProcessTileMap();
                TileMap tileMap = tmr.CreateTileMap();
                ConfigureTileSetsInMap(tileMap);
                CreateMap(mapCode, tileMap);
            }
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();         
        }

        public override void Update(GameTime gameTime)
        {
            if (!_gameRef.GamePlayScreen.IsPaused)
            {
                base.Update(gameTime);
                CurrentMap.Update(gameTime);
            }            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            CurrentMap.Draw(spriteBatch, gameTime, camera);
        }
        public void SetCurrentMap(EMapCode mapCode)
        {
            if (_maps.ContainsKey(mapCode))
            {
                _currentMap = mapCode;
            }
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

        private void CreateMap(EMapCode mapCode, TileMap tileMap)
        {
            Map map = new Map(tileMap, _debugTexture);
            _maps.Add(mapCode, map);
        }
        #endregion
    }
}
