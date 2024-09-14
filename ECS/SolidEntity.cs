using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class SolidEntity : Entity {

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

    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, Color.Red * 0.5f);
    }
}