using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class PlayerEntity : GameActorEntity {
    
    // Events
    public event EventHandler PlayerKilled;

    // Enums
    public enum MoveState
    {
        Normal,
        Climbing
    }

    // Constants

    // Events
    public event EventHandler HeadBonked;
    public MoveState MovingState;

    // Properties

    // Constructors
    public PlayerEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0, string textureAssetId = "") 
        : base(collisionWidth, collisionHeight, startX, startY, textureAssetId)
    {

        // Player-Specific Components
        AddComponent(
            new PhysicsComponent()
        );

        AddComponent(
            new AnimationComponent(
                GetComponent<SpriteComponent>(),
                new Rectangle(50, 38, 50, 37),
                6, 0.05f, AnimationComponent.Loop.Reverse
            )
        );

        AddComponent(
            new InputComponent()
        );

        AddComponent(
            new HealthComponent(3)
        );

        // Set Defaults
        MovingState = MoveState.Normal;

        // Connect our ColliderComponent Collided event to the MoveComponent collision correction method
        GetComponent<ColliderComponent>().Collided += GetComponent<MoveComponent>().OnCollision;
        GetComponent<ColliderComponent>().Collided += OnCollision;
        GetComponent<HealthComponent>().Killed += OnKilled;
    }

    // Methods
    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Yellow * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().BottomCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().LeftCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().RightCollider, Color.Red * 0.5f);
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().TopCollider, Color.Red * 0.5f);
    }

    public override void Update() {
        MoveComponent movement = GetComponent<MoveComponent>();
        ColliderComponent collider = GetComponent<ColliderComponent>();
        InputComponent input = GetComponent<InputComponent>();
        PhysicsComponent physics = GetComponent<PhysicsComponent>();

        // Check for Input
        if(input.IsRunKeyDown && !movement.IsRunning) {
            movement.MaxMoveSpeed *= 2f;
            movement.CurrentAcceleration *= 1.25f;
            movement.IsRunning = true;
        }

        if(!input.IsRunKeyDown && movement.IsRunning) {
            movement.MaxMoveSpeed = movement.BaseMoveSpeed;
            movement.CurrentAcceleration = movement.BaseAcceleration;
            movement.IsRunning = false;
        }

        // Check for Horizontal Key presses (A & D) and apply Horizontal Velocity, as needed
        if(input.IsLeftKeyDown && MovingState != MoveState.Climbing) {
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.LeftCollider) != null)
            {
                movement.Velocity = new Vector2(0, movement.Velocity.Y);
                movement.CurrentMoveSpeed = 0;
            }
            else 
            {
                if(movement.CurrentMoveSpeed < movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed += movement.CurrentAcceleration;
                }
                else if (movement.CurrentMoveSpeed > movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed = movement.MaxMoveSpeed;
                }

                if(movement.CurrentAcceleration < movement.MinimumMoveSpeed) {
                    movement.CurrentAcceleration = movement.MinimumMoveSpeed;
                }

                float velocityX = (float)-(Math.Floor((movement.Velocity.X + movement.CurrentMoveSpeed) * Globals.DeltaTime));

                movement.Velocity = new Vector2(velocityX, movement.Velocity.Y);
            }
        }

        if(input.IsRightKeyDown && MovingState != MoveState.Climbing) {

            // If the ColliderSystem returns true, we are against a wall on our right already
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.RightCollider) != null) {
                // Set horizontal velocity to 0, we will not move if there is a wall in our way
                movement.Velocity = new Vector2(0, movement.Velocity.Y);
                movement.CurrentMoveSpeed = 0;
                
            } 
            else {
                // If no collisions are detected, we are free to begin moving
                if(movement.CurrentMoveSpeed < movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed += movement.CurrentAcceleration;
                }
                else if (movement.CurrentMoveSpeed > movement.MaxMoveSpeed) {
                    movement.CurrentMoveSpeed = movement.MaxMoveSpeed;
                }

                if(movement.CurrentAcceleration < movement.MinimumMoveSpeed) {
                    movement.CurrentAcceleration = movement.MinimumMoveSpeed;
                }

                float velocityX = (movement.Velocity.X + movement.CurrentMoveSpeed) * Globals.DeltaTime;

                movement.Velocity = new Vector2(velocityX, movement.Velocity.Y);
            }
        }

        if(!input.IsLeftKeyDown && !input.IsRightKeyDown) {
            movement.Velocity = new Vector2(0, movement.Velocity.Y);
            movement.CurrentMoveSpeed = 0;
        }

        // Check for Jump Key while On the Ground
        if(input.IsJumpKeyDown) 
        {
            if(MovingState != MoveState.Climbing && physics.OnGround) 
            {
                physics.Jump();
            }
            else if(MovingState == MoveState.Climbing)
            {
                GetComponent<PhysicsComponent>().Enable();
                physics.Jump();
                MovingState = MoveState.Normal;
            }
        }

        if((input.IsUpKeyDown || input.IsDownKeyDown) && ColliderSystem.CheckForEntityCollision<LadderEntity>(collider.Collider) != null && MovingState != MoveState.Climbing)
        {
            GetComponent<PhysicsComponent>().Disable();
            LadderEntity ladder = ColliderSystem.CheckForEntityCollision<LadderEntity>(collider.Collider);
            movement.Velocity = Vector2.Zero;
            GetComponent<TransformComponent>().Position = 
                new Vector2(
                    (int)ladder.GetComponent<TransformComponent>().Position.X, 
                    GetComponent<TransformComponent>().Position.Y
                );
            MovingState = MoveState.Climbing;
        }

        if(input.IsUpKeyDown && MovingState == MoveState.Climbing) 
        {
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.TopCollider) == null)
            {
                movement.Velocity = new Vector2(0, -(movement.BaseMoveSpeed * Globals.DeltaTime));
            }
        }

        if(input.IsDownKeyDown && MovingState == MoveState.Climbing) 
        {
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.BottomCollider) != null)
            {
                GetComponent<PhysicsComponent>().Enable();
                MovingState = MoveState.Normal;
            }
            else
            {
                movement.Velocity = new Vector2(0, movement.BaseMoveSpeed * Globals.DeltaTime);
            }
        }

        if(!input.IsDownKeyDown && !input.IsUpKeyDown && MovingState == MoveState.Climbing)
        {
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.TopCollider) == null)
            {
                movement.Velocity = Vector2.Zero;
            }
        }

        // End Jump Premature if Jump Key is Released before reaching max jump height
        if(!input.IsJumpKeyDown && !physics.OnGround && movement.Velocity.Y < 0 && physics.IsBouncing == false && MovingState == MoveState.Normal) {
            movement.Velocity = new Vector2(movement.Velocity.X, 0);
        }

        // Move with Moving Platform
        var platformEntity = ColliderSystem.CheckForEntityCollision<MovingPlatformEntity>(collider.BottomCollider);
        if(platformEntity != null) {
            // We are on a moving platform here
            Vector2 platformVelocity = platformEntity.GetComponent<MoveComponent>().Velocity;
            movement.Velocity += platformVelocity;
        }

        // TODO: Adjust SpriteEffects and AnimationComponent based on Movement

        // Update the AnimationComponent's DestinationRectangle
        GetComponent<AnimationComponent>().DestinationRectangle = GetComponent<ColliderComponent>().Collider;
    }

    private void MoveAndSlide()
    {

    }

    protected void OnHeadBonked() {
        HeadBonked?.Invoke(this, null);
    }

    protected override void OnCollision(object sender, CollisionEventArgs args) 
    {
        Entity entity = args.CollisionComponent.Entity;

        if(entity.GetType() == typeof(EnemyEntity)) {

            int bounceCheck = Math.Abs(GetComponent<ColliderComponent>().Collider.Bottom - args.CollisionComponent.Collider.Top);

            if(bounceCheck < 8)
            {
                // We hit the enemy's head, so we deal damage
                // Bounce on Head if we hit his head
                var enemy = entity as EnemyEntity;
                enemy.GetComponent<HealthComponent>().Damage();

                GetComponent<PhysicsComponent>().OnGround = true;
                GetComponent<PhysicsComponent>().Bounce();
            }
            else
            {
                // Take Damage
                GetComponent<HealthComponent>().Damage(1);
            }
            
        }
    }

    protected void OnKilled(object sender, EventArgs args)
    {
        PlayerKilled?.Invoke(this, null);
    }
}