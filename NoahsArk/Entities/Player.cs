using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Sprites;
using NoahsArk.Levels;
using NoahsArk.Levels.Maps;
using NoahsArk.Rendering;
using NoahsArk.Utilities;

namespace NoahsArk.Entities
{
    public class Player : Entity
    {
        #region Fields
        private PlayerIndex _playerIndex;
        private Vector2 _desiredMovement;
        private World _world;
        private bool _isTransitioningMaps;
        private float _transitionCooldown;
        private const float _transition_cooldown_duration = 0.5f;
        private bool _isInteracting;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Player(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed, 
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations, Camera camera, PlayerIndex playerIndex,
            Texture2D shadow, World world) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, shadow, camera)
        {
            _playerIndex = playerIndex;
            _world = world;
            _transitionCooldown = 0f;
            _isInteracting = false;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            UpdateTransitionCooldownTimer(gameTime);
            HandleCameraControls();
            HandleMovement();
            if (!_isTransitioningMaps)
            {
                HandleDoorTransitions();
            }            
            if (_desiredMovement != Vector2.Zero)
            {
                Move(_desiredMovement);
                LockToMap();
                if (Camera.CameraMode == ECameraMode.Follow)
                {
                    Camera.LockToPosition(Position, CurrentMap);
                }
            }
            UpdateInteractionState();
            base.Update(gameTime);
        }

        public override Circle GetHitbox(Vector2 desiredMovement)
        {
            Vector2 feetPosition = desiredMovement + new Vector2(8, 24); // 16px = bottom half, add another 8px to be at center of bottom 16px = 24px
            return new Circle(feetPosition, 10f); // radius of 8 makes a circle 16px wide
        }
        #endregion

        #region Private
        private void UpdateTransitionCooldownTimer(GameTime gameTime)
        {
            if (_transitionCooldown > 0)
            {
                _transitionCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_transitionCooldown <= 0)
                {
                    _transitionCooldown = 0;
                    _isTransitioningMaps = false; // allow new transitions after cooldown
                }
            }
        }
        private void UpdateInteractionState()
        {
            if (InputHandler.KeyReleased(Keys.E) || 
                InputHandler.ButtonReleased(Buttons.A, _playerIndex))
            {
                _isInteracting = false;
            }
        }
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
                _desiredMovement = direction * Speed * isRunningSpeed;                
            } else
            {
                SetAnimation(EAnimationKey.Idle, facingDirection);
                _desiredMovement = Vector2.Zero;
            }
        }
        private void HandleDoorTransitions()
        {
            if (_isInteracting || _transitionCooldown > 0)
            {
                return; // dont handle door transitions
            }
            
            if (InputHandler.KeyPressed(Keys.E) || 
                InputHandler.ButtonPressed(Buttons.A, _playerIndex))
            {
                _isInteracting = true;
                Circle playerHitbox = GetHitbox(Position);
                for (int i = 0; i < CurrentMap.TileMap.Doors.Count; i++)
                {
                    DoorTransition door = CurrentMap.TileMap.Doors[i];
                    if (door.Direction == CurrentDirection &&
                        CollisionHelper.CircleIntersectsRectangle(playerHitbox, door.TriggerArea))
                    {
                        TransitionThroughDoor(door.TargetMap, door.SpawnPosition);
                        break;
                    }
                }
            }
        }
        private void TransitionThroughDoor(EMapCode targetMap, Vector2 playerSpawnPosition)
        {
            _isTransitioningMaps = true;
            _desiredMovement = Vector2.Zero;
            EMapCode targetMapCode = targetMap;
            CurrentMap.RemovePlayer(this);
            _world.SetCurrentMap(targetMapCode);
            CurrentMap = _world.CurrentMap;
            CurrentMap.AddPlayer(this);
            Position = playerSpawnPosition;
            Camera.LockToPosition(Position, CurrentMap);
            _transitionCooldown = _transition_cooldown_duration; // start the transition cooldown
        }
        #endregion
    }
}
