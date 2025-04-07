using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoahsArk.Managers;

namespace NoahsArk.States
{
    public abstract class GameState : DrawableGameComponent
    {
        #region Fields
        private List<GameComponent> _childComponents;
        protected GameStateManager _gameStateManager;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        protected GameState(Game game, GameStateManager manager) : base(game)
        {
            _gameStateManager = manager;
            _childComponents = new List<GameComponent>();
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            DrawableGameComponent drawComponent;
            for (int i = 0; i < _childComponents.Count; i++)
            {
                GameComponent component = _childComponents[i];
                if (component is DrawableGameComponent)
                {
                    drawComponent = component as DrawableGameComponent;
                    if (drawComponent.Enabled)
                    {
                        drawComponent.Draw(gameTime);
                    }
                }
            }
            base.Update(gameTime);
        }

        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (_gameStateManager.CurrentState == this)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;
            for (int i = 0; i < _childComponents.Count; i++)
            {
                GameComponent component = _childComponents[i];
                component.Enabled = true;
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = true;
                }
            }
        }

        protected virtual void Hide()
        {
            Visible = false;
            Enabled = false;
            for (int i = 0; i < _childComponents.Count; i++)
            {
                GameComponent component = _childComponents[i];
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = false;
                }
            }
        }
        #endregion

    }
}
