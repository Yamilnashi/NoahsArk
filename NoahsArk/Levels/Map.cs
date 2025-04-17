using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Entities;
using NoahsArk.Levels.Maps;
using NoahsArk.Rendering;

namespace NoahsArk.Levels
{
    public class Map
    {
        #region Fields
        private EMapCode _mapCode;
        private TileMap _tileMap;
        private List<Player> _players = new List<Player>();
        private List<Enemy> _enemies = new List<Enemy>(); 
        // characters
        // enemies
        #endregion

        #region Properties
        public List<Player> Players { get { return _players; } }
        public List<Enemy> Enemies { get { return _enemies; } }
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
            for (int i = 0; i < _players.Count; i++)
            {
                Player player = _players[i];
                player.Update(gameTime);
            }

            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                enemy.Update(gameTime);
            }

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
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                enemy.Draw(spriteBatch);
            }

            // draw character
            for (int i = 0; i < _players.Count; i++)
            {
                Player player = _players[i];
                player.Draw(spriteBatch);
            }

            for (int i = 0; i <  layersToDrawAfterCharacter.Count; i++)
            {
                ILayer layer = layersToDrawAfterCharacter[i];
                layer.Draw(spriteBatch, gameTime, camera, _tileMap.TileSets);
            }

            // draw collisions
        }
        public void AddPlayer(Player player)
        {
            _players.Add(player);
            player.CurrentMap = this;
        }
        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            player.CurrentMap = null;
        }
        public void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
            enemy.CurrentMap = this;
        }
        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
            enemy.CurrentMap = null;    
        }
        
        #endregion
    }
}
