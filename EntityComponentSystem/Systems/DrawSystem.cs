using Microsoft.Xna.Framework.Graphics;

namespace VaniaPlatformer.ECS;

public class DrawSystem : BaseSystem<SpriteComponent> 
{
    // Methods
    public static void Draw(SpriteBatch spriteBatch) {
        
        foreach(SpriteComponent c in components) {
            if(c.Enabled)
            {
                c.Draw(spriteBatch);
            }
        }
        
    }
}