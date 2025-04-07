using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoahsArk.States;

namespace NoahsArk.Managers
{
    public class GameStateManager : GameComponent
    {
        #region Events
        public event EventHandler OnStateChange;
        #endregion

        #region Fields
        private Stack<GameState> _gameStates = new Stack<GameState>();
        private const int _startDrawOrder = 5000;
        private const int _drawOrderInc = 100;
        private int _drawOrder;
        #endregion

        #region Properties
        public GameState CurrentState
        {
            get
            {
                return _gameStates.Peek();
            }
        }
        #endregion

        #region Constructor
        public GameStateManager(Game game) : base(game)
        {
            _drawOrder = _startDrawOrder;
        }
        #endregion

        #region Methods
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public void PopState()
        {
            if (_gameStates.Count > 0)
            {
                RemoveState();
                _drawOrder -= _drawOrderInc;
                if (OnStateChange != null)
                {
                    OnStateChange(this, null);
                }
            }
        }

        public void PushState(GameState newState)
        {
            _drawOrder += _drawOrderInc;
            newState.DrawOrder = _drawOrder;
            AddState(newState);
            if (OnStateChange != null)
            {
                OnStateChange(this, null);
            }
        }
        public void ChangeState(GameState newState)
        {
            while(_gameStates.Count > 0)
            {
                RemoveState();
            }
            newState.DrawOrder = _startDrawOrder;
            _drawOrder = _startDrawOrder;
            AddState(newState);

            if (OnStateChange != null)
            {
                OnStateChange(this, null);
            }
        }
        
        #endregion

        #region Private
        private void RemoveState()
        {
            GameState gameState = CurrentState;
            OnStateChange -= gameState.StateChange;
            Game.Components.Remove(gameState);
            _gameStates.Pop();
        }
        private void AddState(GameState newState)
        {
            _gameStates.Push(newState);
            Game.Components.Add(newState);
            OnStateChange += newState.StateChange;
        }
        #endregion

    }
}
