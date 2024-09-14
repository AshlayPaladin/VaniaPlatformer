using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class MoveComponent : Component 
{
    // Fields
    private float coyoteTimer;

    // Properties
    public Vector2 Velocity = Vector2.Zero;
    public float BaseMoveSpeed = 128.0f;
    public float MinimumMoveSpeed = 16f;
    public float CurrentMoveSpeed = 0f;
    public float MaxMoveSpeed = 128.0f;
    public float CoyoteTime = 0.15f;
    public float BaseAcceleration = 8f;
    public float CurrentAcceleration = 0f;
    public float JumpSpeed = 420.0f;
    public float MaxFallSpeed = 128.0f;
    public float EffectiveGravity = 0.35f;
    public bool IsRunning = false;
    public bool OnGround = false;

    // Constructor
    public MoveComponent()
    {
        this.coyoteTimer = CoyoteTime;

        MoveSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        TransformComponent transform = Entity.GetComponent<TransformComponent>();
        ColliderComponent collider = Entity.GetComponent<ColliderComponent>();

        // Add Gravity to Falling, if we are in the air
        if(!OnGround)
        {
            float currentFallSpeed = Velocity.Y;

            if(currentFallSpeed < MaxFallSpeed)
            {
                currentFallSpeed += EffectiveGravity;

                Velocity = new Vector2(Velocity.X, currentFallSpeed);
            }
        }
        else
        {
            if(collider != null)
            {
                if(!ColliderSystem.CheckForAnyCollision(collider.BottomCollider))
                {
                    // We're set as OnGround, but nothing is under us, Begin CoyoteTime!
                    // Nothing below us, begin Coyote Time countdown before setting OnGround
                    if(coyoteTimer > 0) {
                        coyoteTimer -= Globals.DeltaTime;
                    }
                    else {
                        OnGround = false;
                        coyoteTimer = CoyoteTime;
                    }

                    // Begin falling immediately, but still be considered "onGround" so player can still jump
                    float currentFallSpeed = Velocity.Y;

                    if(currentFallSpeed < MaxFallSpeed) {
                        currentFallSpeed += EffectiveGravity;

                        Velocity = new Vector2(Velocity.X, currentFallSpeed);
                    }
                }
                else {
                    coyoteTimer = CoyoteTime;
                }
            }
        }

        // Collisions have been Handled, Falling Applied, Let's Apply Velocity
        if(Velocity.X != 0 || Velocity.Y != 0)
        {
            transform.Position = new Vector2(
                transform.Position.X + Velocity.X,
                transform.Position.Y + Velocity.Y
            );

            if(collider != null) 
            {
                collider.Collider = new Rectangle(
                    (int)transform.Position.X,
                    (int)transform.Position.Y,
                    collider.Collider.Width,
                    collider.Collider.Height
                );
            }

            var sprite = Entity.GetComponent<SpriteComponent>();
            if(sprite != null)
            {
                if(Velocity.X < 0)
                {
                    sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    sprite.SpriteEffect = SpriteEffects.None;
                }
            }
        }
    }

    public void Stop() 
    {
        CurrentMoveSpeed = 0f;
    }

    public void Accelerate() 
    {
        if(CurrentMoveSpeed < MaxMoveSpeed) 
        {
            CurrentMoveSpeed += CurrentAcceleration;
        }
    }

    public void Decelerate() 
    {
        if(CurrentMoveSpeed > 0) 
        {
            CurrentMoveSpeed /= 1.15f;
        }
    }

    public void Jump() 
    {
        float jumpVelocity = -(JumpSpeed * Globals.DeltaTime);

        Velocity = new Vector2(Velocity.X, jumpVelocity);
        
        OnGround = false;
    }

    public void OnCollision(object o, CollisionEventArgs args) 
    {
        var transform = Entity.GetComponent<TransformComponent>();
        var playerCollider = Entity.GetComponent<ColliderComponent>().Collider;

        if(args.CollisionRectangle.Height >= args.CollisionRectangle.Width) 
        {
            float leftDiff = Math.Abs(args.CollisionRectangle.Left - playerCollider.Left);
            float rightDiff = Math.Abs(args.CollisionRectangle.Right - playerCollider.Right);

            if(leftDiff <= rightDiff) {
                // Collision is on our LEFT side
                float collisionProposedX = transform.Position.X + args.CollisionRectangle.Width;

                transform.Position = new Vector2(collisionProposedX, transform.Position.Y);
            } 
            else if (rightDiff < leftDiff) {
                // Collision is on our RIGHT side
                float collisionProposedX = transform.Position.X - args.CollisionRectangle.Width;

                transform.Position = new Vector2(collisionProposedX, transform.Position.Y);
            }

            Velocity = new Vector2(0, Velocity.Y);
        }
        else 
        {
            float topDiff = Math.Abs(args.CollisionRectangle.Top - playerCollider.Top);
            float bottomDiff = Math.Abs(args.CollisionRectangle.Bottom - playerCollider.Bottom);

            if(bottomDiff <= topDiff) {
                // Collision is BELOW Player
                float collisionProposedY = transform.Position.Y - args.CollisionRectangle.Height;

                transform.Position = new Vector2(transform.Position.X, collisionProposedY);

                OnGround = true;
            }
            else if (topDiff < bottomDiff) {
                // Collision is ABOVE Player
                float collisionProposedY = transform.Position.Y + args.CollisionRectangle.Height;

                transform.Position = new Vector2(transform.Position.X, collisionProposedY);

                OnGround = false;
            }

            Velocity = new Vector2(Velocity.X, 0);
        }

        var collider = Entity.GetComponent<ColliderComponent>();
        if(collider != null) 
        {
            collider.Collider = new Rectangle(
                (int)transform.Position.X,
                (int)transform.Position.Y,
                collider.Collider.Width,
                collider.Collider.Height
            );
        }
    }
}