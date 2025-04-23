using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.Players;
using NoahsArk.Entities.Sprites;
using NoahsArk.Levels;
using NoahsArk.Levels.Maps;
using NoahsArk.Managers;
using NoahsArk.Rendering;

namespace NoahsArk.States
{
    public class GamePlayScreen : BaseGameState
    {
        #region Fields
        private Engine _engine = new Engine(16, 16);
        private World _world;
        private Camera _camera;
        private Texture2D _debugTexture;
        #endregion

        #region Properties
        public Engine Engine { get { return _engine; } }
        public Camera Camera { get { return _camera; } }
        #endregion

        #region Constructor
        public GamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            _camera = new Camera(_gameRef.ScreenRectangle);
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            LoadWorld();
            CreatePlayers();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _world.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, transformMatrix: _camera.Transformation);
            base.Draw(gameTime);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _world.Draw(_gameRef.SpriteBatch, gameTime, _camera);
            _gameRef.SpriteBatch.End();
        }
        #endregion

        #region Private
        private void LoadWorld()
        {
            _debugTexture = new Texture2D(_gameRef.GraphicsDevice, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
            _world = new World(_gameRef, _debugTexture);
            _world.SetCurrentMap(EMapCode.HomeOutside);
            Game.Components.Add(_world);
        }
        private void CreatePlayers()
        {
            string playerDataFilePath = Path.Combine(_gameRef.Content.RootDirectory, "Assets/GameData/Players/player-data.json");
            string jsonContent = File.ReadAllText(playerDataFilePath);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(jsonContent);
            PlayerObject p = playerData.PlayerObjects.FirstOrDefault(x => x.ClassType == EClassType.Rogue);
            Vector2 initialPosition = new Vector2(10, 10);
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations = GetAnimationData(p.Animations);
            Texture2D shadow = _gameRef.Content.Load<Texture2D>("Assets/Sprites/Character/shadow");
            var player = new Player(p.HealthPoints, p.ManaPoints, initialPosition, p.Speed, animations, _camera, PlayerIndex.One, shadow, _world);
            _world.CurrentMap.AddPlayer(player);
        }
        private Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> GetAnimationData(Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> data)
        {
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations = new Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>>();
            for (int i = 0; i < data.Count; i++)
            {
                EAnimationKey key = data.Keys.ElementAt(i);
                animations.Add(key, new Dictionary<EDirection, AnimatedSprite>());
                for (int j = 0; j < data[key].Count; j++)
                {
                    EDirection direction = data[key].Keys.ElementAt(j);
                    AnimationData animationData = data[key][direction];
                    string formattedFilePath = GetFormattedFilePath(animationData.TextureFilePath);
                    Texture2D texture = _gameRef.Content.Load<Texture2D>(formattedFilePath);
                    AnimatedSprite sprite = new AnimatedSprite(texture, animationData.FrameCount,
                        animationData.FrameWidth, animationData.FrameHeight, animationData.FrameDuration);
                    animations[key].Add(direction, sprite);
                }                
            }
            return animations;
        }
        private string GetFormattedFilePath(string filePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension); ;
        }
        #endregion
    }
}
