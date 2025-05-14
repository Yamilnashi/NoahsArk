using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public class BaseGameState : GameState
    {
        #region Fields
        protected Game1 _gameRef;
        protected ControlManager _controlManager;
        protected PlayerIndex _playerIndex;
        protected BaseGameState _transitionTo;
        protected bool _isTransitioning;
        protected EChangeType _changeType;
        protected TimeSpan _transitionTimer;
        protected TimeSpan _transitionInterval = TimeSpan.FromSeconds(0.5);
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public BaseGameState(Game game, GameStateManager manager) : base(game, manager)
        {
            _gameRef = (Game1)game;
            _playerIndex = PlayerIndex.One;
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            ContentManager Content = Game.Content;
            Dictionary<(string font, int size), SpriteFont> spriteFontsDict = new Dictionary<(string font, int size), SpriteFont>()
            {
                {("Silver", 28), Content.Load<SpriteFont>("Assets/Fonts/Silver28")},
                {("Monogram", 18), Content.Load<SpriteFont>("Assets/Fonts/Monogram18")},
                {("Monogram", 16), Content.Load<SpriteFont>("Assets/Fonts/Monogram16")},
                {("Monogram", 14), Content.Load<SpriteFont>("Assets/Fonts/Monogram14")},
                {("Monogram", 12), Content.Load<SpriteFont>("Assets/Fonts/Monogram12")},
                {("Monogram", 10), Content.Load<SpriteFont>("Assets/Fonts/Monogram10")}
            };
            _controlManager = new ControlManager(spriteFontsDict);
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            if (_isTransitioning)
            {
                _transitionTimer += gameTime.ElapsedGameTime;
                if (_transitionTimer >= _transitionInterval)
                {
                    _isTransitioning = false;
                    switch (_changeType)
                    {
                        case EChangeType.Change:
                            _gameStateManager.ChangeState(_transitionTo);
                            break;
                        case EChangeType.Pop:
                            _gameStateManager.PopState();
                            break;
                        case EChangeType.Push:
                            _gameStateManager.PushState(_transitionTo);
                            break;
                    }
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public virtual void Transition(EChangeType changeType, BaseGameState gameState)
        {
            _isTransitioning = true;
            _changeType = changeType;
            _transitionTo = gameState;
            _transitionTimer = TimeSpan.Zero;
        }
        #endregion
    }
}
