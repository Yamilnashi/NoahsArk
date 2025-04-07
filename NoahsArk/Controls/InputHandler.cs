using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace NoahsArk.Controls
{
    public class InputHandler : GameComponent
    {

        #region Keyboard Fields
        private static KeyboardState keyboardState;
        private static KeyboardState lastKeyboardState;
        #endregion

        #region Mouse Region
        private static MouseState mouseState;
        private static MouseState lastMouseState;
        #endregion

        #region Game Pad Fields
        private static GamePadState[] gamePadStates;
        private static GamePadState[] lastGamePadStates;
        #endregion

        #region Keyboard Properties
        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }
        #endregion

        #region Mouse Properties
        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }
        #endregion

        #region Game Pad Properties
        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }
        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }
        #endregion

        #region Constructor
        public InputHandler(Game game) : base(game)
        {
            keyboardState = Keyboard.GetState();
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            mouseState = Mouse.GetState();

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                gamePadStates[(int)index] = GamePad.GetState(index);
            }
        }
        #endregion

        #region Monogame Methods
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                gamePadStates[(int)index] = GamePad.GetState(index);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Methods
        public static void Flush()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
        }
        #endregion

        #region Keyboard Methods
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }
        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        #endregion

        #region Mouse Methods
        public static Point MouseAsPoint
        {
            get { return new Point(mouseState.X, mouseState.Y); }
        }

        public static Vector2 MouseAsVector2
        {
            get { return new Vector2(mouseState.X, mouseState.Y); }
        }

        public static Point LastMouseAsPoint
        {
            get { return new Point(lastMouseState.X, lastMouseState.Y); }
        }

        public static Vector2 LastMouseAsVector2
        {
            get { return new Vector2(lastMouseState.X, lastMouseState.Y); }
        }

        public static bool CheckMousePress(EMouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case EMouseButton.Left:
                    result = mouseState.LeftButton == ButtonState.Pressed &&
                        lastMouseState.LeftButton == ButtonState.Released;
                    break;
                case EMouseButton.Right:
                    result = mouseState.RightButton == ButtonState.Pressed &&
                        lastMouseState.RightButton == ButtonState.Released;
                    break;
                case EMouseButton.Middle:
                    result = mouseState.MiddleButton == ButtonState.Pressed &&
                        lastMouseState.MiddleButton == ButtonState.Released;
                    break;
            }
            return result;
        }

        public static bool CheckMouseReleased(EMouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case EMouseButton.Left:
                    result = mouseState.LeftButton == ButtonState.Released &&
                        lastMouseState.LeftButton == ButtonState.Pressed;
                    break;
                case EMouseButton.Right:
                    result = mouseState.RightButton == ButtonState.Released &&
                        lastMouseState.RightButton == ButtonState.Pressed;
                    break;
                case EMouseButton.Middle:
                    result = mouseState.MiddleButton == ButtonState.Released &&
                        lastMouseState.MiddleButton == ButtonState.Pressed;
                    break;
            }

            return result;
        }

        public static bool IsMouseDown(EMouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case EMouseButton.Left:
                    result = mouseState.LeftButton == ButtonState.Pressed;
                    break;
                case EMouseButton.Right:
                    result = mouseState.RightButton == ButtonState.Pressed;
                    break;
                case EMouseButton.Middle:
                    result = mouseState.MiddleButton == ButtonState.Pressed;
                    break;
            }

            return result;
        }

        public static bool IsMouseUp(EMouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case EMouseButton.Left:
                    result = mouseState.LeftButton == ButtonState.Released;
                    break;
                case EMouseButton.Right:
                    result = mouseState.RightButton == ButtonState.Released;
                    break;
                case EMouseButton.Middle:
                    result = mouseState.MiddleButton == ButtonState.Released;
                    break;
            }

            return result;
        }
        #endregion

        #region Game Pad Methods
        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
            lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
            lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }
        #endregion

    }
}
