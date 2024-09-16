using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class LadderEntity : Entity
{
    // Properties

    // Constructor
    public LadderEntity(int collisionWidth, int collisionHeight, int startX = 0, int startY = 0)
    {
        AddComponent(
            new TransformComponent(
                new Vector2(startX, startY),
                new Vector2(1, 1)
            )
        );

        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    startX, startY, collisionWidth, collisionHeight
                )
            )
        );
    }
}