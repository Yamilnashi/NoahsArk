using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using NoahsArk.Rendering;

namespace NoahsArk.Entities
{
    public class Player : Entity
    {
        #region Fields
        private PlayerIndex _playerIndex;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Player(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed, 
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations, Camera camera, PlayerIndex playerIndex) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, camera)
        {
            _playerIndex = playerIndex;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            HandleCameraControls();
            HandleMovement();
            base.Update(gameTime);
        }
        #endregion

        #region Private
        private void HandleCameraControls()
        {
            if (InputHandler.KeyReleased(Keys.PageUp) ||
                InputHandler.ButtonReleased(Buttons.LeftShoulder, PlayerIndex.One))
            {
                Camera.ZoomIn(CurrentMap);
                if (Camera.CameraMode == ECameraMode.Follow)
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            }
            else if (InputHandler.KeyReleased(Keys.PageDown) ||
                InputHandler.ButtonReleased(Buttons.RightShoulder, PlayerIndex.One))
            {
                Camera.ZoomOut(CurrentMap);
                if (Camera.CameraMode == ECameraMode.Follow)
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            }

            if (InputHandler.KeyReleased(Keys.F) ||
                InputHandler.ButtonReleased(Buttons.RightStick, PlayerIndex.One))
            {
                Camera.ToggleCameraMode();

                if (Camera.CameraMode == ECameraMode.Follow)
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            }

            if (Camera.CameraMode != ECameraMode.Follow)
            {
                if (InputHandler.KeyReleased(Keys.C) ||
                    InputHandler.ButtonReleased(Buttons.LeftStick, PlayerIndex.One))
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            }
        }
        private void HandleMovement()
        {
            GamePadState gamePadState = GamePad.GetState(_playerIndex);
            KeyboardState keyboardState = Keyboard.GetState();           
            EDirection facingDirection = CurrentDirection;
            EAnimationKey animationState = EAnimationKey.Idle;
            Vector2 direction = Vector2.Zero;
            float isRunningSpeed = 1f;
            if (gamePadState.DPad.Up == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up))
            {
                direction.Y -= 1;
                facingDirection = EDirection.Up;
                animationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Right == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.D) ||
                keyboardState.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
                facingDirection = EDirection.Right;
                animationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Down == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.Down))
            {
                direction.Y += 1;
                facingDirection = EDirection.Down;
                animationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Left == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
                facingDirection = EDirection.Left;
                animationState = EAnimationKey.Walking;
            }

            if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                animationState = EAnimationKey.Running;
                isRunningSpeed = 1.8f;
            }

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                SetAnimation(animationState, facingDirection);                
                Move(direction * Speed * isRunningSpeed);
                LockToMap();
                if (Camera.CameraMode == ECameraMode.Follow)
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            } else
            {
                SetAnimation(EAnimationKey.Idle, facingDirection);
            }
        }
        #endregion
    }
}
