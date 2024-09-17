using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    // Fields
    private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();

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
            new RigidBodyComponent()
        );

        AddComponent(
            new AnimationComponent()
        );

        AddComponent(
            new InputComponent()
        );

        AddComponent(
            new HealthComponent(3)
        );

        // Set Defaults
        MovingState = MoveState.Normal;

        GetComponent<AnimationComponent>().Animations.Add(
            new Animation("Idle", new Rectangle(0, 0, 32, 32), 2, 0.5f, Animation.Loop.FromBeginning)
        );

        GetComponent<AnimationComponent>().Animations.Add(
            new Animation("Walk", new Rectangle(0, 65, 32, 32), 4, 0.15f, Animation.Loop.FromBeginning)
        );

        GetComponent<AnimationComponent>().Animations.Add(
            new Animation("Run", new Rectangle(0, 97, 32, 32), 8, 0.05f, Animation.Loop.FromBeginning)
        );

        // Connect our ColliderComponent Collided event to the MovementComponent collision correction method
        GetComponent<ColliderComponent>().Collided += GetComponent<MovementComponent>().OnCollision;
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
        MovementComponent movement = GetComponent<MovementComponent>();
        ColliderComponent collider = GetComponent<ColliderComponent>();
        InputComponent input = GetComponent<InputComponent>();
        RigidBodyComponent physics = GetComponent<RigidBodyComponent>();

        // Update Movememnt maximums if we begin running
        if(input.IsRunKeyDown && !movement.IsRunning) {
            movement.MaxMoveSpeed *= 2f;
            movement.CurrentAcceleration *= 1.25f;
            movement.IsRunning = true;
        }

        // Reset Movement maximums if we are no longer running
        if(!input.IsRunKeyDown && movement.IsRunning) {
            movement.MaxMoveSpeed = movement.BaseMoveSpeed;
            movement.CurrentAcceleration = movement.BaseAcceleration;
            movement.IsRunning = false;
        }

        // Check for LEFT input while not Climbing
        if(input.IsLeftKeyDown && MovingState != MoveState.Climbing) {

            if(movement.IsRunning && GetComponent<AnimationComponent>().Animation != animations["Run"])
            {
                AnimationSystem.Play(this, "Run");
            }
            if(!movement.IsRunning && GetComponent<AnimationComponent>().Animation != animations["Walk"])
            {
                AnimationSystem.Play(this, "Walk");
            }

            // If the following returns anything other than NULL, we are against a SolidEntity on our Left,
            // so we don't proceed with movement.
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

        // Check for RIGHT input while not Climbing
        if(input.IsRightKeyDown && MovingState != MoveState.Climbing) {

            if(movement.IsRunning && GetComponent<AnimationComponent>().Animation != animations["Run"])
            {
                AnimationSystem.Play(this, "Run");
            }
            if(!movement.IsRunning && GetComponent<AnimationComponent>().Animation != animations["Walk"])
            {
                AnimationSystem.Play(this, "Walk");
            }

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

        // If LEFT and RIGHT are both not being pressed, we stop horizontal velocity
        // TODO: Update this with deceleration, rather than hard-setting Velocity to 0
        if(!input.IsLeftKeyDown && !input.IsRightKeyDown) {
            movement.Velocity = new Vector2(0, movement.Velocity.Y);
            movement.CurrentMoveSpeed = 0;

            var anim = GetComponent<AnimationComponent>();
            if(anim.Animation.Name != "Idle")
            {
                AnimationSystem.Play(this, "Idle");
            }
        }

        // Check for Jump Key
        if(input.IsJumpKeyDown) 
        {
            // If we are moving normally, the Jump key only works if we're on the ground
            if(MovingState == MoveState.Normal && physics.OnGround) 
            {
                physics.Jump();
            }
            // If we are climing, the Jump key always works and re-enables Physics, such as Gravity
            else if(MovingState == MoveState.Climbing)
            {
                GetComponent<RigidBodyComponent>().Enable();
                physics.Jump();
                physics.OnGround = false;
                MovingState = MoveState.Normal;
            }
        }

        // Ladder Check -- Check for UP or DOWN input, any collision with a LadderEntity, and that we aren't already climbing
        if((input.IsUpKeyDown || input.IsDownKeyDown) && 
            ColliderSystem.CheckForEntityCollision<LadderEntity>(collider.Collider) != null && 
            MovingState != MoveState.Climbing)
        {
            GetComponent<RigidBodyComponent>().Disable();
            LadderEntity ladder = ColliderSystem.CheckForEntityCollision<LadderEntity>(collider.Collider);
            movement.Velocity = Vector2.Zero;
            GetComponent<TransformComponent>().Position = 
                new Vector2(
                    (int)ladder.GetComponent<TransformComponent>().Position.X, 
                    GetComponent<TransformComponent>().Position.Y
                );
            MovingState = MoveState.Climbing;
        }

        // While climbing, UP and DOWN slide us up and down the Ladder
        if(input.IsUpKeyDown && MovingState == MoveState.Climbing) 
        {

            // TODO: We need to make sure that the player does not leave the LadderEntity while climbing
            // This current check ONLY checks if are going to hit a solid and stops us if so, it does
            // not prevent us from leaving the Ladder itself
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.TopCollider) == null)
            {
                movement.Velocity = new Vector2(0, -(movement.BaseMoveSpeed * Globals.DeltaTime));
            }
        }

        // While climbing, UP and DOWN slide us up and down the Ladder
        if(input.IsDownKeyDown && MovingState == MoveState.Climbing) 
        {

            // TODO: We need to make sure that the player does not leave the LadderEntity while climbing
            // This current check ONLY checks if are going to hit a solid and stops us if so, it does
            // not prevent us from leaving the Ladder itself
            if(ColliderSystem.CheckForEntityCollision<SolidEntity>(collider.BottomCollider) != null)
            {
                GetComponent<RigidBodyComponent>().Enable();
                MovingState = MoveState.Normal;
            }
            else
            {
                movement.Velocity = new Vector2(0, movement.BaseMoveSpeed * Globals.DeltaTime);
            }
        }

        // If we release UP and DOWN while on a Ladder, we need to reset the Velocity
        if(!input.IsDownKeyDown && !input.IsUpKeyDown && MovingState == MoveState.Climbing)
        {
            movement.Velocity = Vector2.Zero;
        }

        /*  End Jump Premature if Jump Key is Released before reaching max jump height
            Requires the following:
                - Jump key is NOT pressed
                - Player is NOT on the ground
                - Player Velocity shows UPWARD movement
                - We are not bouncing (e.g. off an enemy head)
                - Player MoveState is Normal
        */  
        if(!input.IsJumpKeyDown && 
            !physics.OnGround && 
            movement.Velocity.Y < 0 && 
            physics.IsBouncing == false && 
            MovingState == MoveState.Normal) {
            movement.Velocity = new Vector2(movement.Velocity.X, 0);
        }

        // Apply MovingPlatformEntity Velocity to our own, if we are standing on one
        var platformEntity = ColliderSystem.CheckForEntityCollision<MovingPlatformEntity>(collider.BottomCollider);
        if(platformEntity != null) {
            
            Vector2 platformVelocity = platformEntity.GetComponent<MovementComponent>().Velocity;
            movement.Velocity += platformVelocity;

        }

        // TODO: Adjust SpriteEffects and AnimationComponent based on Movement

        // Update the AnimationComponent's DestinationRectangle
        GetComponent<AnimationComponent>().DestinationRectangle = GetComponent<ColliderComponent>().Collider;
    }

    // Player-specific collision handling. Checks for EnemyEntity and handles damage
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

                GetComponent<RigidBodyComponent>().OnGround = true;
                GetComponent<RigidBodyComponent>().Bounce();
            }
            else
            {
                // Take Damage
                GetComponent<HealthComponent>().Damage(1);
            }
            
        }
    }

    protected void OnHeadBonked() 
    {
        HeadBonked?.Invoke(this, null);
    }

    protected void OnKilled(object sender, EventArgs args)
    {
        PlayerKilled?.Invoke(this, null);
    }
}