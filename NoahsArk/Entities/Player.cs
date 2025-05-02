using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Sprites;
using NoahsArk.Levels;
using NoahsArk.Levels.Maps;
using NoahsArk.Rendering;
using NoahsArk.States;
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
        private float _attackingCooldown;
        private const float _attacking_cooldown_duration = 2.4f;
        private bool _isInteracting;
        private bool _hasAppliedDamage;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Player(int maxHealthPoints, int maxManaPoints, Vector2 initialPosition, float speed,
            Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> animations, Camera camera, PlayerIndex playerIndex,
            Texture2D shadow, World world) : base(maxHealthPoints, maxManaPoints, initialPosition, speed, animations, shadow, camera)
        {
            _playerIndex = playerIndex;
            _world = world;
            _transitionCooldown = 0f;
            _attackingCooldown = 0f;
            _isInteracting = false;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            UpdateTransitionCooldownTimer(gameTime);
            HandleCameraControls();
            HandleMovement();
            UpdateAttackCooldownTimer(gameTime);

            if (!IsAttacking)
            {
                HandleAttack();
            }            

            if (IsAttacking && // we are attacking 
                !_hasAppliedDamage && // we haven't applied damage yet
                Animations[CurrentAnimation][CurrentDirection][EEquipmentSlot.Gloves].IsHitFrame) // at the hit frame                
            {
                ApplyDamage();
                _hasAppliedDamage = true;
            }

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
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (GamePlayScreen.IsDebugEnabled)
            {
                Circle attackhitbox = GetAttackHitbox();
                Rectangle boundingRect = new Rectangle(
                    (int)(attackhitbox.Center.X - attackhitbox.Radius),
                    (int)(attackhitbox.Center.Y - attackhitbox.Radius),
                    (int)(2 * attackhitbox.Radius),
                    (int)(2 * attackhitbox.Radius)
                );
                spriteBatch.Draw(GamePlayScreen.DebugTexture, boundingRect, Color.Yellow * 0.7f);
            }            
        }
        public override Circle GetHitbox(Vector2 desiredMovement)
        {
            Vector2 feetPosition = desiredMovement + new Vector2(8, 24); // 16px = bottom half, add another 8px to be at center of bottom 16px = 24px
            return new Circle(feetPosition, 10f); // radius of 8 makes a circle 16px wide
        }
        #endregion

        #region Private
        private Circle GetAttackHitbox()
        {
            Vector2 attackOffset;
            switch (CurrentDirection)
            {
                case EDirection.Right:
                    attackOffset = new Vector2(26, 24);
                    break;
                case EDirection.Left:
                    attackOffset = new Vector2(-10, 24);
                    break;
                case EDirection.Up:
                    attackOffset = new Vector2(8, 6);
                    break;
                case EDirection.Down:
                    attackOffset = new Vector2(8, 42);
                    break;
                default:
                    attackOffset = Vector2.Zero;
                    break;
            }

            Vector2 attackPosition = Position + attackOffset;
            return new Circle(attackPosition, 8f);
        }
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
        private void UpdateAttackCooldownTimer(GameTime gameTime)
        {
            if (_attackingCooldown > 0)
            {
                _attackingCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_attackingCooldown <= 0)
                {
                    _attackingCooldown = 0;
                    IsAttacking = false;
                    for (int i = 0; i < Animations[CurrentAnimation][CurrentDirection].Keys.Count; i++)
                    {
                        EEquipmentSlot slot = Animations[CurrentAnimation][CurrentDirection].Keys.ElementAt(i);
                        Animations[CurrentAnimation][CurrentDirection][slot].Reset();
                    }
                    SetAnimation(EAnimationKey.Idle, CurrentDirection);
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
            EDirection newFacingDirection = CurrentDirection;
            EAnimationKey newAnimationState = CurrentAnimation;
            Vector2 direction = Vector2.Zero;
            _desiredMovement = Vector2.Zero;
            float speedMultiplier = 1f;
            if (gamePadState.DPad.Up == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up))
            {
                direction.Y -= 1;
                newFacingDirection = EDirection.Up;
                newAnimationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Right == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.D) ||
                keyboardState.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
                newFacingDirection = EDirection.Right;
                newAnimationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Down == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.Down))
            {
                direction.Y += 1;
                newFacingDirection = EDirection.Down;
                newAnimationState = EAnimationKey.Walking;
            }

            if (gamePadState.DPad.Left == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
                newFacingDirection = EDirection.Left;
                newAnimationState = EAnimationKey.Walking;
            }

            if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                newAnimationState = EAnimationKey.Running;
                speedMultiplier = 1.8f;
            }

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                if (IsAttacking)
                {
                    newAnimationState = CurrentAnimation;
                    newFacingDirection = CurrentDirection;
                    speedMultiplier = 0.5f;
                }                
                SetAnimation(newAnimationState, newFacingDirection);
                _desiredMovement = direction * Speed * speedMultiplier;
                return;
            }
            if (!IsAttacking)
            {
                SetAnimation(EAnimationKey.Idle, newFacingDirection);
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
        private void HandleAttack()
        {
            if (InputHandler.KeyPressed(Keys.E) ||
                InputHandler.ButtonPressed(Buttons.X, _playerIndex) ||
                InputHandler.CheckMousePress(EMouseButton.Left))
            {                
                PerformAttack();
                
            }
        }
        private void PerformAttack()
        {
            IsAttacking = true;
            SetAnimation(EAnimationKey.Pierce, CurrentDirection);
            _hasAppliedDamage = false;
            _attackingCooldown = _attacking_cooldown_duration; // start the attack cooldown
        }
        private void ApplyDamage()
        {
            Circle attackHitBox = GetAttackHitbox();

            for (int i = 0; i < CurrentMap.Entities.Count; i++)
            {
                Entity entity = CurrentMap.Entities[i];
                if (entity == this)
                {
                    continue;
                }
                if (!entity.IsDying &&
                    CollisionHelper.CircleIntersectsCircle(attackHitBox, entity.GetHitbox(entity.Position)))
                {
                    DealDamage(entity);
                }
            }
        }
        #endregion
    }
}
