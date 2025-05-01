using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;

namespace NoahsArk.Managers
{
    public class ControlManager : List<Control>
    {
        #region Fields
        private int _selectedControl = 0;
        private static Dictionary<(string font, int size), SpriteFont> _spriteFontsDict = new Dictionary<(string font, int size), SpriteFont>();
        private bool _acceptInput = true;
        #endregion

        #region Events
        public event EventHandler FocusChanged;
        #endregion

        #region Properties        
        #endregion

        #region Constructor
        public ControlManager(Dictionary<(string font, int size), SpriteFont> spriteFontsDict) : base()
        {
            _spriteFontsDict = spriteFontsDict;
        }
        #endregion

        #region Methods
        public static SpriteFont SpriteFont(string font, int size)
        {
            if (_spriteFontsDict.TryGetValue((font, size), out SpriteFont spriteFont))
            {
                return spriteFont;
            }
            throw new InvalidOperationException($"Sprite Font: {font} with size: {size} does not exist.");
        }
        public void Update(GameTime gameTime, PlayerIndex playerIndex)
        {
            if (this.Count == 0)
            {
                return;
            }

            for (int i = 0; i < this.Count; i++)
            {
                Control control = this[i];
                if (control.IsEnabled)
                {
                    control.Update(gameTime);
                }
                if (control.HasFocus)
                {
                    control.HandleInput(playerIndex);
                }
            }

            if (!_acceptInput)
            {
                return;
            }

            if (InputHandler.ButtonPressed(Buttons.LeftThumbstickUp, playerIndex) ||
                InputHandler.ButtonPressed(Buttons.DPadUp, playerIndex) ||
                InputHandler.KeyPressed(Keys.Up) ||
                InputHandler.ButtonPressed(Buttons.LeftThumbstickLeft, playerIndex) ||
                InputHandler.ButtonPressed(Buttons.DPadLeft, playerIndex) ||
                InputHandler.KeyPressed(Keys.Left) ||
                InputHandler.KeyPressed(Keys.A) ||
                InputHandler.KeyPressed(Keys.W))
            {
                PreviousControl();
            }

            if (InputHandler.ButtonPressed(Buttons.LeftThumbstickDown, playerIndex) ||
                InputHandler.ButtonPressed(Buttons.DPadDown, playerIndex) ||
                InputHandler.KeyPressed(Keys.Down) ||
                InputHandler.ButtonPressed(Buttons.LeftThumbstickRight, playerIndex) ||
                InputHandler.ButtonPressed(Buttons.DPadRight, playerIndex) ||
                InputHandler.KeyPressed(Keys.Right) ||
                InputHandler.KeyPressed(Keys.D) ||
                InputHandler.KeyPressed(Keys.S))
            {
                NextControl();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 mouse = InputHandler.MouseAsVector2;
            for (int i = 0; i < this.Count;  i++)
            {
                Control control = this[i];
                if (control.IsVisible)
                {
                    control.Draw(spriteBatch);
                    if (control.GetBounds().Contains(mouse) &&
                        control.TabStop &&
                        control.IsEnabled)
                    {
                        ResetFocusOnControls();
                        control.HasFocus = true;
                        _selectedControl = i;
                        if (FocusChanged != null)
                        {
                            FocusChanged(control, null); // move arrows
                        }
                    }
                }
            }
        }
        public void NextControl()
        {
            if (this.Count == 0)
            {
                return;
            }

            int currentControl = _selectedControl;
            ResetFocusOnControls(); // Reset focus for all controls

            do
            {
                _selectedControl++;
                if (_selectedControl == this.Count)
                {
                    _selectedControl = 0;
                }

                if (this[_selectedControl].TabStop &&
                    this[_selectedControl].IsEnabled)
                {
                    if (FocusChanged != null)
                    {
                        FocusChanged(this[_selectedControl], null);
                    }
                    break;
                }
            } while (currentControl != _selectedControl);

            this[_selectedControl].HasFocus = true;
        }
        public void PreviousControl()
        {
            if (this.Count == 0)
            {
                return;
            }

            int currentControl = _selectedControl;
            ResetFocusOnControls(); // Reset focus for all controls

            do
            {
                _selectedControl--;
                if (_selectedControl < 0)
                {
                    _selectedControl = this.Count - 1; // go to the end
                }

                if (this[_selectedControl].TabStop &&
                    this[_selectedControl].IsEnabled)
                {
                    if (FocusChanged != null)
                    {
                        FocusChanged(this[_selectedControl], null);
                    }
                    break;
                }
            } while (currentControl != _selectedControl);

            this[_selectedControl].HasFocus = true;
        }
        #endregion

        #region Private
        private void ResetFocusOnControls()
        {
            for (int i = 0; i < this.Count; i++)
            {
                Control control = this[i];
                control.HasFocus = false;
            }
        }
        #endregion
    }
}
