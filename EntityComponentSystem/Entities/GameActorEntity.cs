using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class GameActorEntity : Entity
{
    // Enum
    protected enum MoveDirection
    {
        Left,
        Right
    }

    // Fields
    protected string textureAssetId;
    protected MoveDirection direction;

    // Constructor
    public GameActorEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0, string textureAssetId = "")
    {
        Random rnd = new Random();
        direction = rnd.Next() % 2 == 0 ? MoveDirection.Left : MoveDirection.Right;

        AddComponent(
            new TransformComponent(
                new Vector2(startX, startY),
                new Vector2(1, 1)
            )
        );

        AddComponent(
            new SpriteComponent(
                Globals.Content.Load<Texture2D>(textureAssetId)
            )
        );

        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    startX,
                    startY,
                    collisionWidth,
                    collisionHeight
                )
            )
        );

        AddComponent(
            new MovementComponent()
        );
    }

    // Methods
    public virtual void Update() 
    { 
        var movement = GetComponent<MovementComponent>();

        if(movement.CurrentMoveSpeed > 0) 
        {
            GetComponent<SpriteComponent>().SpriteEffect = SpriteEffects.None;
        }
        else
        {
            GetComponent<SpriteComponent>().SpriteEffect = SpriteEffects.FlipHorizontally;
        }

        if(direction == MoveDirection.Left)
        {
            movement.CurrentMoveSpeed = movement.BaseMoveSpeed * Globals.DeltaTime;
        }
        else
        {
            movement.CurrentMoveSpeed = -(movement.BaseMoveSpeed  * Globals.DeltaTime);
        }

        movement.Velocity = new Vector2(movement.CurrentMoveSpeed, movement.Velocity.Y);
    }

    protected virtual void OnCollision(object sender, CollisionEventArgs args)
    {
        if(args.CollisionComponent.Entity.GetType() == typeof(SolidEntity))
        {
            direction = direction == MoveDirection.Left ? MoveDirection.Right : MoveDirection.Left;
        }
    }

    protected virtual void Dispose()
    {
        foreach(Component component in Components)
        {
            var health = GetComponent<HealthComponent>();
            var sprite = GetComponent<SpriteComponent>();
            var animation = GetComponent<AnimationComponent>();
            var collider = GetComponent<ColliderComponent>();

            if(collider != null) 
            {
                ColliderSystem.Deregister(collider);
                collider = null;
            }

            if(animation != null)
            {
                AnimationSystem.Deregister(animation);
                animation = null;
            }

            if(sprite != null)
            {
                DrawSystem.Deregister(sprite);
                sprite = null;
            }

            if(health != null)
            {
                HealthSystem.Deregister(health);
                health = null;
            }
        }
    }
}