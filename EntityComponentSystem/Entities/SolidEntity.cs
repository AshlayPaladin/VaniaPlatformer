using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class SolidEntity : Entity {

    // Fields
    private Color drawColor;

    // Constructor
    public SolidEntity(Vector2 position, int width, int height) {


        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    width,
                    height
                )
            )
        );

        drawColor = Color.Red;
    }

    public SolidEntity(Vector2 position, int width, int height, Circle circleCollider) {


        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    width,
                    height
                )
            )
        );

        drawColor = Color.Red;
    }

    public SolidEntity(Vector2 position, int width, int height, Triangle triangleCollider) {


        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    width,
                    height
                )
            )
        );

        drawColor = Color.Red;
    }

    public SolidEntity(Vector2 position, int width, int height, Capsule capsuleCollider) {


        AddComponent(
            new ColliderComponent(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    width,
                    height
                )
            )
        );

        drawColor = Color.Red;
    }

    public void Draw(SpriteBatch spriteBatch) {
        //spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, drawColor * 0.5f);
    }
}