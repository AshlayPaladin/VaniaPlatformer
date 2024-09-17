using System;
using Microsoft.Xna.Framework;

namespace VaniaPlatformer.ECS;

public class ColliderComponent : Component {

    // Enum
    public enum ColliderShape
    {
        Box,
        Circle,
        Triangle,
        Capsule
    }

    public enum CollisionSide
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }

    // Events
    public event EventHandler<CollisionEventArgs> Collided;

    // Properties
    public ColliderShape Shape { get; private set; }
    public Rectangle BoxCollider;
    public Circle CircleCollider;
    public Triangle TriangleCollider;
    public Capsule CapsuleCollider;
    public Vector2 Correction;

    // Constructor
    public ColliderComponent(Rectangle collider) 
    {
        Shape = ColliderShape.Box;
        BoxCollider = collider;
        Correction = Vector2.Zero;

        ColliderSystem.Register(this);
    }

    public ColliderComponent(Circle collider)
    {
        Shape = ColliderShape.Circle;
        CircleCollider = collider;
        Correction = Vector2.Zero;

        ColliderSystem.Register(this);
    }

    // Triangle Collider is a unique situation where we need both a Triangle and Box collider so we can snap
    // entities to the slope, if needed
    public ColliderComponent(Triangle collider, Rectangle boxCollider)
    {
        Shape = ColliderShape.Triangle;
        TriangleCollider = collider;
        BoxCollider = boxCollider;
        Correction = Vector2.Zero;

        ColliderSystem.Register(this);
    }

    public ColliderComponent(Capsule collider)
    {
        Shape = ColliderShape.Capsule;
        CapsuleCollider = collider;
        Correction = Vector2.Zero;

        ColliderSystem.Register(this);
    }
}