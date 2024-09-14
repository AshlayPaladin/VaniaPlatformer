using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer;
using VaniaPlatformer.ECS;

public class EnemyEntity : Entity
{
    // Enum
    private enum MoveDirection {
        Left,
        Right
    }

    // Fields
    private MoveDirection Direction;
    private string textureAssetId = "textures/TestEnemy";

    // Properties

    // Construction
    public EnemyEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0) 
    {
        Random rnd = new Random();
        
        if(rnd.Next() % 2 == 0)
        {
            Direction = MoveDirection.Left;
        }
        else
        {
            Direction = MoveDirection.Right;
        }

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
                new Rectangle(startX, startY, collisionWidth, collisionHeight)
            )
        );

        AddComponent(
            new MoveComponent()
        );

        GetComponent<ColliderComponent>().Collided += GetComponent<MoveComponent>().OnCollision;
        GetComponent<ColliderComponent>().Collided += OnCollision;
    }

    public void Update() 
    {
        var movement = GetComponent<MoveComponent>();

        if(movement.CurrentMoveSpeed > 0) 
        {
            GetComponent<SpriteComponent>().SpriteEffect = SpriteEffects.None;
        }
        else
        {
            GetComponent<SpriteComponent>().SpriteEffect = SpriteEffects.FlipHorizontally;
        }

        if(Direction == MoveDirection.Left)
        {
            movement.CurrentMoveSpeed = movement.BaseMoveSpeed * Globals.DeltaTime;
        }
        else
        {
            movement.CurrentMoveSpeed = -(movement.BaseMoveSpeed  * Globals.DeltaTime);
        }

        movement.Velocity = new Vector2(movement.CurrentMoveSpeed, movement.Velocity.Y);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Red * 0.5f);
    }

    private void OnCollision(object sender, CollisionEventArgs args) 
    {
        if(args.CollisionComponent.Entity.GetType() == typeof(SolidEntity))
        {
            Direction = Direction == MoveDirection.Left ? MoveDirection.Right : MoveDirection.Left;
        }
    }
}