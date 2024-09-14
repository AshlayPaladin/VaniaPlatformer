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
            for(int j = i + 1; j < components.Count; j++) {
                components.ElementAt(i).Intersects(components.ElementAt(j));
            }
        }
    }

    public static bool CheckForAnyCollision(Rectangle collider) 
    {
        foreach(var c in components) 
        {
            if(c.Collider != collider)
            {
                if(c.Collider.Intersects(collider))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool CheckForEntityCollision<T>(Rectangle collider) where T : Entity
    {
        foreach(var c in components)
        {
            if(c.Entity.GetType() == typeof(T))
            {
                if(c.Collider != collider)
                {
                    if(c.Collider.Intersects(collider))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}