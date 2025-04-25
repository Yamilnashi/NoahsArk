using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.GameObjects;
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
        private bool _isDebugEnabled = true;
        private Player _player;
        private PauseMenuScreen _pauseMenuScreen;
        private bool _isPaused = false;
        #endregion

        #region Properties
        public Engine Engine { get { return _engine; } }
        public Camera Camera { get { return _camera; } }
        public World World { get { return _world; } }
        public bool IsPaused { get { return _isPaused; } }  
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
            _pauseMenuScreen = new PauseMenuScreen(_gameRef, _gameStateManager, _player, _camera);
        }
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            if (InputHandler.KeyPressed(Keys.Escape))
            {
                _isPaused = !_isPaused;
            }

            if (_isPaused)
            {
                _pauseMenuScreen.Update(gameTime);
            } else
            {
                
                _world.Update(gameTime);
            }

            if (InputHandler.KeyPressed(Keys.F1))
            {
                _isDebugEnabled = !_isDebugEnabled;
            }
        }
        public override void Draw(GameTime gameTime)
        {           
            _gameRef.SpriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, transformMatrix: _camera.Transformation);
            base.Draw(gameTime);
            _controlManager.Draw(_gameRef.SpriteBatch);
            _world.Draw(_gameRef.SpriteBatch, gameTime, _camera);
            if (_isDebugEnabled)
            {
                Rectangle playerBox = new Rectangle(
                    (int)_player.Position.X,
                    (int)_player.Position.Y,
                    5,
                    5
                );
                Circle playerHitbox = _player.GetHitbox(_player.Position);
                Rectangle playerHitboxRectangle = new Rectangle(
                    (int)playerHitbox.Center.X - (int)playerHitbox.Radius,
                    (int)playerHitbox.Center.Y - (int)playerHitbox.Radius,
                    (int)playerHitbox.Radius * 2,
                    (int)playerHitbox.Radius * 2
                );
                _gameRef.SpriteBatch.Draw(_debugTexture, playerBox, Color.Red * 0.8f);
                _gameRef.SpriteBatch.Draw(_debugTexture, playerHitboxRectangle, Color.Blue * 0.8f);
            }
            _gameRef.SpriteBatch.End();

            if (_isPaused)
            {
                _pauseMenuScreen.Draw(gameTime);
            }

            if (_isDebugEnabled)
            {
                DrawDebug();
            }
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
            Vector2 initialPosition = new Vector2(295, 240);
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations = GetAnimationData(p.Animations);
            Texture2D shadow = _gameRef.Content.Load<Texture2D>("Assets/Sprites/Character/shadow");
            _player = new Player(p.HealthPoints, p.ManaPoints, initialPosition, p.Speed, animations, _camera, PlayerIndex.One, shadow, _world);
            _world.CurrentMap.AddPlayer(_player);
            _camera.LockToPosition(initialPosition, _player.CurrentMap);
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
        private void DrawDebug()
        {
            _gameRef.SpriteBatch.Begin();

            // get some debug stats
            Vector2 playerPosition = _player.Position;
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseScreenPosition = new Vector2(mouseState.X, mouseState.Y);
            Vector2 mouseWorldPosition = Vector2.Transform(mouseScreenPosition, Matrix.Invert(_camera.Transformation));

            // create the debug box
            Vector2 boxPosition = new Vector2(10, 10);
            string debugText = $"Player: ({playerPosition.X:F0}, {playerPosition.Y:F0})\n" +
                $"Mouse: ({mouseWorldPosition.X:F0}, {mouseWorldPosition.Y:F0})";
            Label debugLabel = new Label("Silver", 28, debugText);
            debugLabel.Position = new Vector2(boxPosition.X + 10, boxPosition.Y + 10);
            Rectangle debugBox = new Rectangle(
                (int)boxPosition.X,
                (int)boxPosition.Y,
                (int)debugLabel.Size.X + 20,
                (int)debugLabel.Size.Y + 20
            );
            // draw the box (background)
            _gameRef.SpriteBatch.Draw(_debugTexture, debugBox, Color.Black * 0.8f); // semi-transparent black
            
            // draw the text
            debugLabel.Draw(_gameRef.SpriteBatch);

            _gameRef.SpriteBatch.End();
        }
        #endregion
    }
}
