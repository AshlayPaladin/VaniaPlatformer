using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaniaPlatformer.ECS;

namespace VaniaPlatformer;

public class OneWaySolidEntity : Entity {

    // Fields
    private Color drawColor;

    // Constructor
    public OneWaySolidEntity(Vector2 position, int width, int height) {

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

        drawColor = Color.Green;
        
    }

    public void Draw(SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.DebugTexture, GetComponent<ColliderComponent>().Collider, drawColor * 0.5f);
    }
}