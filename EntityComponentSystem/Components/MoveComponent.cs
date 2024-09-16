using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class MoveComponent : Component 
{
    // Fields
    

    // Properties
    public Vector2 Velocity = Vector2.Zero;
    public float BaseMoveSpeed = 128.0f;
    public float MinimumMoveSpeed = 16f;
    public float CurrentMoveSpeed = 0f;
    public float MaxMoveSpeed = 128.0f;
    public float BaseAcceleration = 8f;
    public float CurrentAcceleration = 0f;
    public float JumpSpeed = 420.0f;
    public bool IsRunning = false;

    // Constructor
    public MoveComponent()
    {
        MoveSystem.Register(this);
    }

    // Methods
    public override void Update()
    {
        TransformComponent transform = Entity.GetComponent<TransformComponent>();
        ColliderComponent collider = Entity.GetComponent<ColliderComponent>();

        // Add Gravity to Falling, if we are in the air
        

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

    public void OnCollision(object sender, CollisionEventArgs args) 
    {
        Type entityType = args.CollisionComponent.Entity.GetType();

        if(entityType == typeof(SolidEntity) ||
            entityType == typeof(MovingPlatformEntity) || 
            entityType == typeof(OneWaySolidEntity))
        {
            var transform = Entity.GetComponent<TransformComponent>();
            var myCollider = Entity.GetComponent<ColliderComponent>().Collider;

            if(args.CollisionRectangle.Height >= args.CollisionRectangle.Width) 
            {
                if(entityType == typeof(SolidEntity))
                {
                    float leftDiff = Math.Abs(args.CollisionRectangle.Left - myCollider.Left);
                    float rightDiff = Math.Abs(args.CollisionRectangle.Right - myCollider.Right);

                    if(leftDiff <= rightDiff) {
                        // Collision is on Entity LEFT side
                        float collisionProposedX = transform.Position.X + args.CollisionRectangle.Width;

                        transform.Position = new Vector2(collisionProposedX, transform.Position.Y);
                    } 
                    else if (rightDiff < leftDiff) {
                        // Collision is on Entity RIGHT side
                        float collisionProposedX = transform.Position.X - args.CollisionRectangle.Width;

                        transform.Position = new Vector2(collisionProposedX, transform.Position.Y);
                    }

                    Velocity = new Vector2(0, Velocity.Y);
                }
            }
            else 
            {
                float topDiff = Math.Abs(args.CollisionRectangle.Top - myCollider.Top);
                float bottomDiff = Math.Abs(args.CollisionRectangle.Bottom - myCollider.Bottom);

                if(bottomDiff <= topDiff) {
                    // Collision is BELOW Entity
                    if(entityType == typeof(OneWaySolidEntity))
                    {
                        // Only apply solid characteristics if the OneWay is below us
                        if(Velocity.Y > 0 && Math.Abs(Entity.GetComponent<ColliderComponent>().Collider.Bottom - args.CollisionRectangle.Top) <= 12)
                        {
                            // One-Way Solid
                            float collisionProposedY = transform.Position.Y - args.CollisionRectangle.Height;

                            transform.Position = new Vector2(transform.Position.X, collisionProposedY);

                            if(Entity.GetComponent<PhysicsComponent>() != null)
                            {
                                Entity.GetComponent<PhysicsComponent>().OnGround = true;
                            }
                            
                            Velocity = new Vector2(Velocity.X, 0);
                        }
                    }
                    else
                    {
                        // Other Solid (e.g. MovingPlatform)
                        float collisionProposedY = transform.Position.Y - args.CollisionRectangle.Height;

                        transform.Position = new Vector2(transform.Position.X, collisionProposedY);

                        if(Entity.GetComponent<PhysicsComponent>() != null)
                        {
                            Entity.GetComponent<PhysicsComponent>().OnGround = true;
                        }

                        Velocity = new Vector2(Velocity.X, 0);
                    }
                }
                else if (topDiff < bottomDiff) {
                    // Collision is ABOVE Entity

                    if(entityType == typeof(SolidEntity))
                    {
                        float collisionProposedY = transform.Position.Y + args.CollisionRectangle.Height;

                        transform.Position = new Vector2(transform.Position.X, collisionProposedY);

                        if(Entity.GetComponent<PhysicsComponent>() != null)
                        {
                            Entity.GetComponent<PhysicsComponent>().OnGround = false;
                        }

                        Velocity = new Vector2(Velocity.X, 0);
                    }
                    
                }
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
}