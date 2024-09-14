using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class ColliderSystem : BaseSystem<ColliderComponent> 
{
    // Methods
    public static new void Update() 
    {
        foreach(ColliderComponent collider in components) 
        {
            collider.Update();
        }

        for(int i = 0; i < components.Count - 1; i++) {
            for(int j = 1; j < components.Count; j++) {
                components[i].Intersects(components[j].Collider);
            }
        }
    }
}