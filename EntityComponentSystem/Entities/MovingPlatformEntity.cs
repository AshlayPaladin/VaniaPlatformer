namespace VaniaPlatformer.ECS;

public class MovingPlatformEntity : GameActorEntity
{
    // Constructor
    public MovingPlatformEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0, string textureAssetId = "")
        : base(collisionWidth, collisionHeight, startX, startY, textureAssetId)
    {
        GetComponent<MoveComponent>().IsFlying = true;

        GetComponent<ColliderComponent>().Collided += OnCollision;
    }
}