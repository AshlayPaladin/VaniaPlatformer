using System.Linq;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class ColliderSystem : BaseSystem<ColliderComponent> 
{
    // Methods
    public static new void Update() 
    {
        // Update all Colliders based on their potential new positions
        foreach(ColliderComponent collider in components) 
        {
            collider.Update();
        }

        // Check for any collisions between all ColliderComponents in the system
        for(int i = 0; i < components.Count - 1; i++) {
            for(int j = 1; j < components.Count; j++) {
                components.ElementAt(i).Intersects(components.ElementAt(j));
            }
        }
    }

    public static bool CheckForAnyCollision(Rectangle collider) 
    {
        bool intersects = false;

        foreach(var c in components) 
        {
            if(c.Collider != collider)
            {
                intersects = c.Collider.Intersects(collider);
            }
        }

        return intersects;
    }
}