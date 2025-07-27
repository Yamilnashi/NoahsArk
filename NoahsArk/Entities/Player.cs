using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoahsArk.Controls;
using NoahsArk.Entities.GameObjects;
using NoahsArk.Entities.Items.Weapons;
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
        private float _attacking_cooldown_duration = 2.4f;
        private bool _isInteracting;
        private bool _hasAppliedDamage;
        private EAnimationKey _animationState;
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
            base.Update(gameTime);
            EDirection facingDirection = HandleFacingDirection();
            UpdateTransitionCooldownTimer(gameTime);
            HandleCameraControls();
            HandleMovement();
            UpdateAttackCooldownTimer(gameTime);

            if (!IsAttacking)
            {
                HandleAttack();
            }            

            if (IsAttacking)
            {

            } else
            {
                SetAnimation(_animationState, facingDirection);
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
        protected override float GetSpeedMultiplier()
        {
            if (_animationState == EAnimationKey.Pierce &&
                EquippedItems.TryGetValue(EEquipmentSlot.MainHand, out Item item) &&
                item is WeaponObject weapon)
            {
                return weapon.BaseStats.AttackSpeed;
            }
            return base.GetSpeedMultiplier();
        }
        #endregion

        #region Private
        private EDirection CalculateDirection(Vector2 toMouse)
        {
            if (Math.Abs(toMouse.X) > Math.Abs(toMouse.Y))
            {
                return toMouse.X > 0
                    ? EDirection.Right
                    : EDirection.Left;
            }
            else
            {
                return toMouse.Y > 0
                    ? EDirection.Down
                    : EDirection.Up;
            }
        }
        private Circle GetAttackHitbox()
        {
            Vector2 attackOffset;
            float offset = 0;
            if (EquippedItems.TryGetValue(EEquipmentSlot.MainHand, out Item item) &&
                item != null)
            {
                if (item is WeaponObject weapon)
                {
                    offset = weapon.AttackHitboxOffset;
                }
            }
            switch (CurrentDirection)
            {
                case EDirection.Right:
                    attackOffset = new Vector2(26 + offset, 24);
                    break;
                case EDirection.Left:
                    attackOffset = new Vector2(-10 - offset, 24);
                    break;
                case EDirection.Up:
                    attackOffset = new Vector2(8, 6 - offset);
                    break;
                case EDirection.Down:
                    attackOffset = new Vector2(8, 42 + offset);
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
                    SetAnimation(EAnimationKey.Idle, CurrentDirection, true);
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
        private EDirection HandleFacingDirection()
        {
            GamePadState gamePadState = GamePad.GetState(_playerIndex);
            KeyboardState keyboardState = Keyboard.GetState();
            EDirection facingDirection = CurrentDirection;
            if (gamePadState.DPad.Up == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up))
            {
                facingDirection = EDirection.Up;
            }

            if (gamePadState.DPad.Right == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.D) ||
                keyboardState.IsKeyDown(Keys.Right))
            {
                facingDirection = EDirection.Right;
            }

            if (gamePadState.DPad.Down == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.Down))
            {
                facingDirection = EDirection.Down;
            }

            if (gamePadState.DPad.Left == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                facingDirection = EDirection.Left;
            }
            return facingDirection;
        }
        private void HandleMovement()
        {
            GamePadState gamePadState = GamePad.GetState(_playerIndex);
            KeyboardState keyboardState = Keyboard.GetState();           
            Vector2 direction = Vector2.Zero;
            float speedMultiplier = 1f;
            if (gamePadState.DPad.Up == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up))
            {
                direction.Y -= 1;
            }

            if (gamePadState.DPad.Right == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.D) ||
                keyboardState.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
            }

            if (gamePadState.DPad.Down == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.S) ||
                keyboardState.IsKeyDown(Keys.Down))
            {
                direction.Y += 1;
            }

            if (gamePadState.DPad.Left == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
            }

            if (!IsAttacking)
            {
                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.LeftShift) &&
                direction != Vector2.Zero)
                {
                    _animationState = EAnimationKey.Running;
                    speedMultiplier = 1.8f;
                }
                else if (direction != Vector2.Zero)
                {
                    _animationState = EAnimationKey.Walking;
                }
                else
                {
                    _animationState = EAnimationKey.Idle;
                }
            }            

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                if (IsAttacking)
                {
                    speedMultiplier = 0.5f;
                }
                _desiredMovement = direction * Speed * speedMultiplier;
            } else
            {
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
        private void HandleAttack()
        {
            if (InputHandler.ButtonPressed(Buttons.X, _playerIndex) ||
                InputHandler.CheckMousePress(EMouseButton.Left))
            {                
                PerformAttack();                
            }
        }
        private void PerformAttack()
        {
            Vector2 mouseScreenPosition = InputHandler.MouseAsVector2;
            Vector2 mouseWorldPosition = Vector2.Transform(mouseScreenPosition, Matrix.Invert(Camera.Transformation));
            Vector2 toMouse = mouseWorldPosition - GetHitbox(Position).Center;
            EDirection facingDirection = CalculateDirection(toMouse);

            IsAttacking = true;
            _animationState = EAnimationKey.Pierce;
            SetAnimation(EAnimationKey.Pierce, facingDirection, false);
            _hasAppliedDamage = false;
            CalculateAnimationTimeBasedOnSpeed(facingDirection);
            
        }
        private void CalculateAnimationTimeBasedOnSpeed(EDirection facingDirection)
        {
            float speedMultiplier = GetSpeedMultiplier();
            int frameCount = Animations[EAnimationKey.Pierce][facingDirection][EEquipmentSlot.MainHand].TotalFrames;
            float baseFrameDuration = Animations[EAnimationKey.Pierce][facingDirection][EEquipmentSlot.MainHand].FrameDurection;
            _attacking_cooldown_duration = (frameCount * baseFrameDuration) / speedMultiplier / 1000f; // convert ms to seconds
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

                Circle entityHitbox = entity.GetHitbox(entity.Position);

                if (!entity.IsDying &&
                    CollisionHelper.PlayerIntersectsCircle(attackHitBox, entityHitbox, out Vector2 contactPoint))
                {
                    DealDamage(entity, contactPoint);
                }
            }
        }
        #endregion
    }
}
