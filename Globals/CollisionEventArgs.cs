using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class CollisionEventArgs : EventArgs 
{
    // Properties
    public Rectangle CollisionRectangle { get; private set; }

    // Constructor
    public CollisionEventArgs(Rectangle collisionRectangle)
    {
        CollisionRectangle = collisionRectangle;
    }
}