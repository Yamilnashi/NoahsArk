using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.Enemies;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Items;
using NoahsArk.Entities.Items.Weapons;
using NoahsArk.Entities.Players;
using NoahsArk.Extensions;
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
        private Player _player;
        private PauseMenuScreen _pauseMenuScreen;
        private bool _isPaused = false;
        private static ContentManager _contentRef;
        private static GraphicsDevice _graphicsDevice;
        private static Texture2D _debugTexture;
        private static bool _isDebugEnabled = false;        
        private static Dictionary<(EWeaponType, EMaterialType), WeaponObject> _weaponObjectDict = new Dictionary<(EWeaponType, EMaterialType), WeaponObject>();
        private static Dictionary<EEnemyType, Dictionary<ERarity, EnemyEntity>> _enemyEntityDict = new Dictionary<EEnemyType, Dictionary<ERarity, EnemyEntity>>();
        private Texture2D _particleTexture;
        private Random _random = new Random();
        #endregion

        #region Properties
        public Engine Engine { get { return _engine; } }
        public Camera Camera { get { return _camera; } }
        public World World { get { return _world; } }
        public bool IsPaused { get { return _isPaused; } }  
        public static Texture2D DebugTexture { get { return _debugTexture; } }
        public static Dictionary<(EWeaponType, EMaterialType), WeaponObject> WeaponObjectDict { get  { return _weaponObjectDict; } }
        public static Dictionary<EEnemyType, Dictionary<ERarity, EnemyEntity>> EnemyEntityDict {  get  { return _enemyEntityDict; } }
        public static ContentManager ContentRef { get { return _contentRef; } }
        public static GraphicsDevice GraphicsDeviceRef { get { return _graphicsDevice; } }
        public static bool IsDebugEnabled { get { return _isDebugEnabled; } }
        #endregion

        #region Constructor
        public GamePlayScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            _camera = new Camera(_gameRef.ScreenRectangle);
            _contentRef = game.Content;
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            base.LoadContent();
            LoadEntities();
            LoadItems();
            LoadWorld();
            LoadEnemies();
            CreatePlayers();            
            _pauseMenuScreen = new PauseMenuScreen(_gameRef, _gameStateManager, _player, _camera);
            _graphicsDevice = _gameRef.GraphicsDevice;
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
        public void SpawnBloodParticles(Vector2 position, int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(_random.NextDouble() * Math.PI * 2 - Math.PI / 2);
                float speed = (float)(_random.NextDouble() * 40 + 10); // adjust for speed hopefulyl gravity has time to act
                Vector2 velocity = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);

                float lifetime = (float)(_random.NextDouble() * 0.8 + 0.6); // 0.6-1.4 sec
                float size = (float)(_random.NextDouble() * 0.6 + 0.4); // 0.4-1 scale
                float rotationSpeed = (float)(_random.NextDouble() * 2 - 1) * MathHelper.Pi; // random spin
                Color gradient = Color.Lerp(Color.DarkRed, Color.Red, (float)_random.NextDouble());
                Particle p = new Particle(_particleTexture, position, velocity, lifetime, gradient, size, rotationSpeed);
                _world.CurrentMap.AddParticle(p);
            }
        }
        #endregion

        #region Private
        private void LoadEntities()
        {
            _particleTexture = _contentRef.Load<Texture2D>("Assets/Sprites/Environment/blood");
        }
        private void LoadItems()
        {
            LoadWeapons();
        }
        private void LoadWeapons()
        {
            string weaponDataFilePath = Path.Combine(_gameRef.Content.RootDirectory, "Assets/GameData/Items/weapon-data.json");
            string jsonContent = File.ReadAllText(weaponDataFilePath);
            WeaponData data = JsonConvert.DeserializeObject<WeaponData>(jsonContent);
            _weaponObjectDict = new Dictionary<(EWeaponType, EMaterialType), WeaponObject>();
            for (int i = 0; i < data.weaponObjects.Count; i++)
            {
                WeaponObject w = data.weaponObjects[i];
                _weaponObjectDict[(w.WeaponType, w.MaterialType)] = w;
            }
        }
        private void LoadWorld()
        {
            _debugTexture = new Texture2D(_gameRef.GraphicsDevice, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
            _world = new World(_gameRef, _debugTexture);
            _world.SetCurrentMap(EMapCode.Development);
            Game.Components.Add(_world);
        }
        private void LoadEnemies()
        {
            string enemyDataFilePath = Path.Combine(_gameRef.Content.RootDirectory, "Assets/GameData/Enemies/enemy-data.json");
            string jsonContent = File.ReadAllText(enemyDataFilePath);
            EnemyData data = JsonConvert.DeserializeObject<EnemyData>(jsonContent);
            Texture2D shadow = _contentRef.Load<Texture2D>("Assets/Sprites/Character/shadow");
            Texture2D rarityMarker = _contentRef.Load<Texture2D>("Assets/Sprites/Enemies/Rarity/marker");
            Enemy.HealthBarTexture = new Dictionary<ERarity, Texture2D>();
            Enemy.HealthBarRectangle = new Dictionary<ERarity, Rectangle>();

            ERarity[] rarities = Enum.GetValues(typeof(ERarity))
                .Cast<ERarity>()
                .ToArray();

            EEnemyType[] enemyTypes = Enum.GetValues(typeof(EEnemyType))
                .Cast<EEnemyType>()
                .ToArray();

            for (int i = 0; i < rarities.Length; i++)
            {
                ERarity r = rarities[i];
                Enemy.HealthBarTexture[r] = GetHealthBarTexture(r);
                Enemy.HealthBarRectangle[r] = GetHealthBarRectangle(r);
            }

            for (int i = 0; i < enemyTypes.Length; i++)
            {
                EEnemyType enemyType = enemyTypes[i];
                _enemyEntityDict[enemyType] = new Dictionary<ERarity, EnemyEntity>();
            }
            
            for (int i = 0; i < data.EnemyObjects.Count; i++)
            {
                EnemyObject obj = data.EnemyObjects[i];
                EnemyEntity entity = new EnemyEntity(obj.EnemyType, obj.HealthPoints, obj.ManaPoints, obj.ExperienceRewardPoints, 
                    obj.Speed, obj.RarityType, obj.Animations, _camera, rarityMarker, shadow);
                if (!_enemyEntityDict[obj.EnemyType].TryGetValue(obj.RarityType, out EnemyEntity found))
                {
                    _enemyEntityDict[obj.EnemyType][obj.RarityType] = entity;
                }                
            }
        }
        private Texture2D GetHealthBarTexture(ERarity r)
        {            
            string healthBarName = r switch
            {
                ERarity.Normal => "healthbar-grey",
                ERarity.Magic => "healthbar-bronze",
                ERarity.Rare => "healthbar-silver",
                ERarity.Epic => "healthbar-gold",
                ERarity.Legendary => "healthbar-red",
                _ => "healthbar-grey"
            };
            return _contentRef.Load<Texture2D>($"Assets/Sprites/Enemies/HealthBar/{healthBarName}");
        }
        private Rectangle GetHealthBarRectangle(ERarity r)
        {
            return new Rectangle(0, 0, 48, 16);
        }
        private void CreatePlayers()
        {
            string playerDataFilePath = Path.Combine(_contentRef.RootDirectory, "Assets/GameData/Players/player-data.json");
            string jsonContent = File.ReadAllText(playerDataFilePath);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(jsonContent);
            PlayerObject p = playerData.PlayerObjects.FirstOrDefault(x => x.ClassType == EClassType.Rogue);
            Vector2 initialPosition = _world.CurrentMapCode.GetInitialPosition();
            Texture2D shadow = _contentRef.Load<Texture2D>("Assets/Sprites/Character/shadow");
            _player = new Player(p.HealthPoints, p.ManaPoints, initialPosition, p.Speed, p.Animations, _camera, PlayerIndex.One, shadow, _world);
            _player.EquipWeapon(EWeaponType.Dagger, EMaterialType.Wooden);
            _world.CurrentMap.AddPlayer(_player);
            _camera.LockToPosition(initialPosition, _player.CurrentMap);
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
