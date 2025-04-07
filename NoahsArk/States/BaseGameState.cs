using System;
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
            SpriteFont menuFont = Content.Load<SpriteFont>("Assets/Fonts/Silver");
            _controlManager = new ControlManager(menuFont);
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
