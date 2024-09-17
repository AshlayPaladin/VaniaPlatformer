using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class CollisionEventArgs : EventArgs 
{
    // Properties
    public ColliderComponent CollisionComponent { get; private set; }
    public Rectangle CollisionRectangle { get; private set; }
    public ColliderComponent.CollisionSide CollisionSide { get; private set; }

    // Constructor
    public CollisionEventArgs(ColliderComponent component, Rectangle collisionRectangle, ColliderComponent.CollisionSide collisionSide)
    {
        CollisionComponent = component;
        CollisionRectangle = collisionRectangle;
        CollisionSide = collisionSide;
    }
}